using SignalRGammon.Backgammon;
using System;

namespace SignalRGammon
{
    internal class GameFactory : IGameFactory
    {
        public IGame CreateGame(string gameType)
        {
            return gameType switch
            {
                "backgammon" => new Backgammon.BackgammonGame(new DieRoller()),
                _ => throw new NotImplementedException()
            };
        }
    }
}