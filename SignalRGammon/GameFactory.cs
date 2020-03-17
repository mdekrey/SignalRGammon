using System;

namespace SignalRGammon
{
    internal class GameFactory : IGameFactory
    {
        public IGame CreateGame(string gameType)
        {
            return gameType switch
            {
                "backgammon" => new Backgammon.BackgammonGame(),
                _ => throw new NotImplementedException()
            };
        }
    }
}