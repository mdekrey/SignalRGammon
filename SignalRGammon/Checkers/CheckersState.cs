using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    public class CheckersState
    {
        public Player? CurrentPlayer { get; private set; }
        public Player? Winner { get; private set; }


        public static CheckersState DefaultState() =>
            new CheckersState()
            {
                CurrentPlayer = null,
                Winner = null,
            };

        public CheckersState With(
            Player? CurrentPlayer = null,
            Player? Winner = null
        )
        {
            return new CheckersState()
            {
                CurrentPlayer = CurrentPlayer ?? this.CurrentPlayer,
                Winner = Winner ?? this.Winner,
            };
        }
    }
}
