using Microsoft.Extensions.Caching.Memory;
using SignalRGame.Clients;
using SignalRGame.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Discovery
{
    public class GameServers : IGameServers
    {
        private readonly IServerDiscovery serverDiscovery;
        private readonly IMemoryCache memoryCache;
        private readonly IGameClientFactory gameClientFactory;

        public GameServers(IServerDiscovery serverDiscovery, IMemoryCache memoryCache, IGameClientFactory gameClientFactory)
        {
            this.serverDiscovery = serverDiscovery;
            this.memoryCache = memoryCache;
            this.gameClientFactory = gameClientFactory;
        }

        public async Task<GameDetails?> CreateGameAsync(string gameType)
        {
            var servers = (await GetServersSupportingGameType(gameType)).ToArray();
            if (servers.Length == 0)
                return null;

            // TODO - better logic here, such as load awareness, etc.
            var gameServer = servers[0];

            var client = gameClientFactory.CreateGameApiClient(gameServer.InternalUrl);
            using var createGameResponse = await client.CreateGameAsync(gameType);
            if (createGameResponse.StatusCode != System.Net.HttpStatusCode.OK)
                return null;
            var game = await createGameResponse.StatusCode200Async();
            return new GameDetails(game, gameServer.PublicUrl);
        }

        public async Task<ServerDetails?> FindGameServerAsync(string id, string gameType)
        {
            foreach (var gameServer in await GetServersSupportingGameType(gameType))
            {
                var client = gameClientFactory.CreateGameApiClient(gameServer.InternalUrl);
                using var findGameResponse = await client.GetGameIdExistsAsync(id);
                return findGameResponse.Response.IsSuccessStatusCode
                    ? (ServerDetails?)gameServer
                    : null;
            }

            return null;
        }

        private async Task<IEnumerable<ServerDetails>> GetServersSupportingGameType(string gameType)
        {
            return from s in await GetTypesForAllServers()
                   where s.types.Select(t => t._GameType).Contains(gameType)
                   select s.server;
        }

        public async Task<IReadOnlyList<Controllers.GameType>> GetAllGameTypesAsync()
        {
            return (from servers in await GetTypesForAllServers()
                    from gameType in servers.types
                    group (IconUrl: servers.server.PublicUrl + gameType.IconUrl, gameType.DisplayName) by gameType._GameType into gameType
                    select new Controllers.GameType(
                        gameType.Key,
                        // TODO - better logic here, such as load awareness, etc.
                        gameType.First().DisplayName,
                        gameType.First().IconUrl
                    ))
                .Distinct()
                .ToArray();
        }

        private async Task<(ServerDetails server, IReadOnlyList<GameType> types)[]> GetTypesForAllServers()
        {
            return await Task.WhenAll(from server in serverDiscovery.GetGameServers()
                                      select AsTask(async () => (server, types: await GetGameTypesByServer(server.InternalUrl))));
        }

        private static Task<T> AsTask<T>(Func<Task<T>> asyncFunc) => asyncFunc();

        private Task<IReadOnlyList<GameType>> GetGameTypesByServer(string server)
        {
            return memoryCache.GetOrCreateAsync(server, async entry =>
            {
                var client = gameClientFactory.CreateGameApiClient(server);
                try
                {
                    using var response = await client.GetGameTypesAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(30));
                        return await response.StatusCode200Async();
                    }
                }
                catch
                {
                    // TODO - log
                }
                // TODO - use poly for a backoff here
                entry.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(60));
                return Array.Empty<GameType>();
            });
        }
    }
}
