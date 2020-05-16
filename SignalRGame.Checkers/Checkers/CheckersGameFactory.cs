using SignalRGame.GameServer;
using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRGame.Checkers
{
    class CheckersGameFactory : ILocalizedGameFactory
    {
        public string Type => "checkers";

        public string DisplayName => "Checkers";

        public string IconUrl => "/images/empty-chessboard.svg";

        public IGame CreateGame() =>
            new Checkers.CheckersGame();
    }
}
