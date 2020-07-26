using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SignalRGame.Backgammon
{
    using static Defaults;
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public record BackgammonState
    {
        public Player? CurrentPlayer { get; init; }
        public Player? Winner { get; init; }
        public DiceState DiceRolls { get; init; } = EmptyDiceRolls;
        public IReadOnlyList<PointState> Points { get; init; } = StartingPosition;
        public PointState Bar { get; init; } = EmptyPoint;
        public BackgammonState? Undo { get; init; }
    }

}