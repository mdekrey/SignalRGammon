using SignalRGame.Backgammon;
using SignalRGame.GameUtilities;
using System;

namespace SignalRGame
{
    internal class _GameFactory : IGameFactory
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