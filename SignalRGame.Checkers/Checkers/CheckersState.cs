using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Checkers
{
    public record SingleChecker
    {
        public int Column { get; init; }
        public int Row { get; init; }
        public bool IsKing { get; init; }

        public static SingleChecker Create(int Column, int Row, bool IsKing = false)
        {
            if ((Column + Row) % 2 == 0)
            {
                throw new InvalidOperationException("Invalid checker placement");
            }
            return new SingleChecker
            {
                Column = Column,
                Row = Row,
                IsKing = IsKing,
            };
        }

        public SingleChecker MoveTo(int Column, int Row)
        {
            return new SingleChecker { Column = Column, Row = Row, IsKing = Row == 7 || Row == 0 || IsKing };
        }
    }

    public record CheckersState
    {
        public static readonly PlayerState<IReadOnlyList<SingleChecker?>> InitialCheckers = new PlayerState<IReadOnlyList<SingleChecker?>>(
            white: new SingleChecker?[]
            {
                SingleChecker.Create(1, 0),
                SingleChecker.Create(3, 0),
                SingleChecker.Create(5, 0),
                SingleChecker.Create(7, 0),
                SingleChecker.Create(0, 1),
                SingleChecker.Create(2, 1),
                SingleChecker.Create(4, 1),
                SingleChecker.Create(6, 1),
                SingleChecker.Create(1, 2),
                SingleChecker.Create(3, 2),
                SingleChecker.Create(5, 2),
                SingleChecker.Create(7, 2),
            },
            black: new SingleChecker?[]
            {
                SingleChecker.Create(0, 7),
                SingleChecker.Create(2, 7),
                SingleChecker.Create(4, 7),
                SingleChecker.Create(6, 7),
                SingleChecker.Create(1, 6),
                SingleChecker.Create(3, 6),
                SingleChecker.Create(5, 6),
                SingleChecker.Create(7, 6),
                SingleChecker.Create(0, 5),
                SingleChecker.Create(2, 5),
                SingleChecker.Create(4, 5),
                SingleChecker.Create(6, 5),
            }
        );

        public Player CurrentPlayer { get; init; } = Player.White;
        public int? MovingChecker { get; init; } = null;
        public Player? Winner { get; init; } = null;
        public PlayerState<bool> IsReady { get; init; } = new PlayerState<bool>(false, false);
        public PlayerState<IReadOnlyList<SingleChecker?>> Checkers { get; init; } = InitialCheckers;
    }
}
