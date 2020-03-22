using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    using static Defaults;
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public struct BackgammonState
    {
        [JsonIgnore]
        public IDieRoller DieRoller { get; private set; }
        public Player? CurrentPlayer { get; private set; }
        public DiceState DiceRolls { get; private set; }
        public IReadOnlyList<PointState> Points { get; private set; }
        public PointState Bar { get; private set; }

        public BackgammonState(IDieRoller dieRoller) : this()
        {
            DieRoller = dieRoller;
        }

        public BackgammonState(BackgammonState original)
        {
            this.DieRoller = original.DieRoller;
            this.CurrentPlayer = original.CurrentPlayer;
            this.DiceRolls = original.DiceRolls;
            this.Points = original.Points;
            this.Bar = original.Bar;
        }

        public static BackgammonState DefaultState(IDieRoller dieRoller) =>
            new BackgammonState
            {
                DieRoller = dieRoller,
                CurrentPlayer = null,
                DiceRolls = Defaults.EmptyDiceRolls,
                Points = StartingPosition,
                Bar = Defaults.EmptyPoint,
            };

        public BackgammonState With(
            Player? CurrentPlayer = null,
            DiceState? DiceRolls = null,
            IReadOnlyList<PointState>? Points = null,
            PointState? Bar = null
        ) =>
            new BackgammonState
            {
                DieRoller = this.DieRoller,
                CurrentPlayer = CurrentPlayer ?? this.CurrentPlayer,
                DiceRolls = DiceRolls ?? this.DiceRolls,
                Points = Points ?? this.Points,
                Bar = Bar ?? this.Bar,
            };

    }

}