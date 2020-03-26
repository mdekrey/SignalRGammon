using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using SignalRGammon.GameUtilities;
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
            var observable = game?.JsonStates ?? Observable.Return<string?>(null);

            return AsSignalRChannel(observable, cancellationToken);
        }

        private ChannelReader<T> AsSignalRChannel<T>(IObservable<T> observable, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<T>();

            observable
                .Aggregate(Task.CompletedTask, (prevTask, next) => prevTask.ContinueWith(t => channel.Writer.WriteAsync(next).AsTask()).Unwrap())
                .Select(next => Observable.FromAsync(() => next))
                .Concat()
                .Subscribe(_ => { }, ex => channel.Writer.Complete(ex), () => channel.Writer.Complete(), cancellationToken);

            return channel.Reader;
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
            return gameId == null ? null : memoryCache.Get<IGame?>(gameId);
        }

    }
}
