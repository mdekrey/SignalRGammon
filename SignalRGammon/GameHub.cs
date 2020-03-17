using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRGammon
{
    public class GameHub : Hub
    {

        private readonly IMemoryCache memoryCache;
        private readonly IGameFactory gameFactory;

        public GameHub(IMemoryCache memoryCache, IGameFactory gameFactory)
        {
            this.memoryCache = memoryCache;
            this.gameFactory = gameFactory;
        }

        public async Task<string> CreateGame(string gameType)
        {
            var gameId = Guid.NewGuid().ToString();
            var game = gameFactory.CreateGame(gameType);
            CreateGame(gameId, game);
            await Task.Yield();
            return gameId;
        }

        public async Task<bool> Do(string gameId, string messageJson)
        {
            var game = GetGame(gameId);
            if (game == null)
            {
                return false;
            }
            return await game.Do(messageJson);
        }


        public ChannelReader<string?> ListenState(string gameId, CancellationToken cancellationToken)
        {
            var game = GetGame(gameId);
            var channel = Channel.CreateUnbounded<string?>();

            // We don't want to await WriteItemsAsync, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItemsAsync(channel.Writer, game, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(
            ChannelWriter<string?> writer,
            IGame? game,
            CancellationToken cancellationToken)
        {
            if (game == null)
            {
                await writer.WriteAsync(null);
                writer.Complete();
                return;
            }
            game.States
                .Do(next => writer.WriteAsync(next))
                .Subscribe(_ => { }, ex => writer.Complete(ex), () => writer.Complete(),  cancellationToken);
        }


        private void CreateGame(string gameId, IGame game)
        {
            memoryCache.GetOrCreate(gameId, cacheEntry =>
            {
                cacheEntry.SlidingExpiration = game.SlidingExpiration;
                return game;
            });
        }

        private IGame? GetGame(string gameId)
        {
            return memoryCache.Get<IGame?>(gameId);
        }

    }
}
