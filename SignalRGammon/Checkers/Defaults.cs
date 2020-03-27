using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    public class Defaults
    {
        public static readonly PlayerState<IReadOnlyList<SingleChecker?>> InitialCheckers = new PlayerState<IReadOnlyList<SingleChecker?>>(
            white: new SingleChecker?[]
            {
                new SingleChecker(1, 0),
                new SingleChecker(3, 0),
                new SingleChecker(5, 0),
                new SingleChecker(7, 0),
                new SingleChecker(0, 1),
                new SingleChecker(2, 1),
                new SingleChecker(4, 1),
                new SingleChecker(6, 1),
                new SingleChecker(1, 2),
                new SingleChecker(3, 2),
                new SingleChecker(5, 2),
                new SingleChecker(7, 2),
            },
            black: new SingleChecker?[]
            {
                new SingleChecker(0, 7),
                new SingleChecker(2, 7),
                new SingleChecker(4, 7),
                new SingleChecker(6, 7),
                new SingleChecker(1, 6),
                new SingleChecker(3, 6),
                new SingleChecker(5, 6),
                new SingleChecker(7, 6),
                new SingleChecker(0, 5),
                new SingleChecker(2, 5),
                new SingleChecker(4, 5),
                new SingleChecker(6, 5),
            }
        );

        public static readonly CheckersState DefaultState = new CheckersState(
                CurrentPlayer: Player.White,
                Winner: null,
                IsReady: new PlayerState<bool>(false, false),
                Checkers: InitialCheckers
            );
    }
}
