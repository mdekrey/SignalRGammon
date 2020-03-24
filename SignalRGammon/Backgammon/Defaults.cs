using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGammon.Backgammon
{
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public static class Defaults
    {
        public static DiceState EmptyDiceRolls = new DiceState(Array.Empty<int>(), Array.Empty<int>());
        public static PointState EmptyPoint = new PointState(0, 0);

        public static IReadOnlyList<PointState> StartingPosition = new[]
        {
            new PointState(black: 2, white: 0),
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            new PointState(white: 5, black: 0),
            EmptyPoint,
            new PointState(white: 3, black: 0),
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            new PointState(black: 5, white: 0),
            new PointState(white: 5, black: 0),
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            new PointState(black: 3, white: 0),
            EmptyPoint,
            new PointState(black: 5, white: 0),
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            EmptyPoint,
            new PointState(white: 2, black: 0),
        }.ToList().AsReadOnly();

    }

}