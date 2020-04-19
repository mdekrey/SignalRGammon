using Microsoft.Extensions.Caching.Memory;
using SignalRGame.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Clients
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
            var servers = (from s in await GetTypesForAllServers()
                           where s.types.Contains(gameType)
                           select s.server).ToArray();
            if (servers.Length == 0)
                return null;

            // TODO - better logic here, such as load awareness, etc.
            var gameServer = servers[0];

            var client = gameClientFactory.CreateGameApiClient(gameServer);
            using var createGameResponse = await client.CreateGameAsync(gameType);
            if (createGameResponse.StatusCode != System.Net.HttpStatusCode.OK)
                return null;
            var game = await createGameResponse.StatusCode200Async();
            return new GameDetails(game, gameServer);
        }

        public async Task<string?> FindGameServerAsync(string id, string gameType)
        {
            foreach (var gameServer in from s in await GetTypesForAllServers()
                                   where s.types.Contains(gameType)
                                   select s.server)
            {
                var client = gameClientFactory.CreateGameApiClient(gameServer);
                using var findGameResponse = await client.GetGameIdExistsAsync(id);
                return findGameResponse.Response.IsSuccessStatusCode
                    ? gameServer
                    : null;
            }

            return null;
        }

        public async Task<IReadOnlyList<string>> GetAllGameTypesAsync()
        {
            return (from servers in await GetTypesForAllServers()
                    from gameType in servers.types
                    select gameType)
                .Distinct()
                .ToArray();
        }

        private async Task<(string server, IReadOnlyList<string> types)[]> GetTypesForAllServers()
        {
            return await Task.WhenAll(from server in serverDiscovery.GetGameServers()
                                      select AsTask(async () => (server, types: await GetGameTypesByServer(server))));
        }

        private static Task<T> AsTask<T>(Func<Task<T>> asyncFunc) => asyncFunc();

        private Task<IReadOnlyList<string>> GetGameTypesByServer(string server)
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
                return Array.Empty<string>();
            });
        }
    }
}
