using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRGame.Backgammon
{
    class BackgammonGameFactory : ISingleGameFactory
    {
        public string Type => "backgammon";

        public IGame CreateGame() =>
            new BackgammonGame(new DieRoller()).CreateInMemoryGame();
    }
}
