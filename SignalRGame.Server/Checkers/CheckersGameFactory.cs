using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRGame.Checkers
{
    class CheckersGameFactory : ISingleGameFactory
    {
        public string Type => "checkers";

        public IGame CreateGame() =>
            new CheckersGame().CreateInMemoryGame();
    }
}
