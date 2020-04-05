using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGame.GameUtilities
{
    public class GameFactory : IGameFactory
    {
        private readonly IEnumerable<ISingleGameFactory> gameFactories;

        public GameFactory(IEnumerable<ISingleGameFactory> gameFactories)
        {
            this.gameFactories = gameFactories;
        }

        public IGame CreateGame(string gameType)
        {
            return gameFactories.FirstOrDefault(factory => factory.Type == gameType)
                ?.CreateGame()
                ?? throw new ArgumentException($"Game type '{gameType}' not registered", nameof(gameType));
        }
    }
}