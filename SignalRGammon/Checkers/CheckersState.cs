using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    public readonly struct SingleChecker
    {
        public int Column { get; }
        public int Row { get; }
        public bool IsKing { get; }

        public SingleChecker(int Column, int Row, bool IsKing = false)
        {
            if ((Column + Row) % 2 == 0)
            {
                throw new InvalidOperationException("Invalid checker placement");
            }
            this.Column = Column;
            this.Row = Row;
            this.IsKing = IsKing;
        }

        public SingleChecker MoveTo(int Column, int Row, bool MakeKing = false)
        {
            return new SingleChecker(Column, Row, IsKing || MakeKing);
        }
    }

    public readonly struct CheckersState
    {
        public Player CurrentPlayer { get; }
        public Player? Winner { get; }
        public PlayerState<bool> IsReady { get; }
        public PlayerState<IReadOnlyList<SingleChecker?>> Checkers { get; }

        public CheckersState(Player CurrentPlayer, Player? Winner, PlayerState<bool> IsReady, PlayerState<IReadOnlyList<SingleChecker?>> Checkers)
        {
            this.CurrentPlayer = CurrentPlayer;
            this.Winner = Winner;
            this.IsReady = IsReady;
            this.Checkers = Checkers;
        }

        public CheckersState With(
            Player? CurrentPlayer = null,
            Player? Winner = null,
            PlayerState<bool>? IsReady = null,
            PlayerState<IReadOnlyList<SingleChecker?>>? Checkers = null
        )
        {
            return new CheckersState(
                CurrentPlayer: CurrentPlayer ?? this.CurrentPlayer,
                Winner: Winner ?? this.Winner,
                IsReady: IsReady ?? this.IsReady,
                Checkers: Checkers ?? this.Checkers
            );
        }
    }
}
