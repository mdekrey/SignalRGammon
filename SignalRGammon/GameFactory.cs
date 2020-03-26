using SignalRGammon.Backgammon;
using SignalRGammon.GameUtilities;
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
                "checkers" => new Checkers.CheckersGame(),
                _ => throw new NotImplementedException()
            };
        }
    }
}