using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SignalRGame.GameUtilities;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRGame
{
    public class GameHub : Hub
    {

        private readonly IMemoryCache memoryCache;
        private readonly IGameFactory gameFactory;
        private readonly ILogger<GameHub> logger;

        public GameHub(IMemoryCache memoryCache, IGameFactory gameFactory, ILogger<GameHub> logger)
        {
            this.memoryCache = memoryCache;
            this.gameFactory = gameFactory;
            this.logger = logger;

            logger.LogInformation("GameHub created");
        }

        public async Task<string> CreateGame(string gameType)
        {
            using (logger.BeginScope("Create game {gameType}", gameType))
            {
                var gameId = Guid.NewGuid().ToString();
                var game = gameFactory.CreateGame(gameType);
                CreateGame(gameId, game);
                await Task.Yield();
                logger.LogInformation("Created {gameType} game with id {gameId}", gameType, gameId);
                return gameId;
            }
        }

        public async Task<bool> Do(string gameId, string messageJson)
        {
            using (logger.BeginScope("Do action on {gameId}", gameId))
            {
                var game = GetGame(gameId);
                if (game == null)
                {
                    logger.LogInformation("Action was rejected");
                    return false;
                }
                logger.LogInformation("Action was accepted");
                return await game.Do(messageJson);
            }
        }

        public ChannelReader<string?> ListenState(string gameId, CancellationToken cancellationToken)
        {
            using (logger.BeginScope("Listen to state on {gameId}", gameId))
            {
                var game = GetGame(gameId);
                var observable = game?.JsonStates ?? Observable.Return<string?>(null);

                return AsSignalRChannel(observable, cancellationToken);
            }
        }

        private ChannelReader<T> AsSignalRChannel<T>(IObservable<T> observable, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<T>();

            observable
                .Aggregate(Task.CompletedTask, (prevTask, next) => prevTask.ContinueWith(t => channel.Writer.WriteAsync(next).AsTask().ContinueWith(_ => logger.LogInformation("Sent state."))).Unwrap())
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
            logger.LogInformation("Added game with id {gameId} to cache", gameId);
        }

        private IGame? GetGame(string gameId)
        {
            return gameId == null ? null : memoryCache.Get<IGame?>(gameId);
        }

    }
}
