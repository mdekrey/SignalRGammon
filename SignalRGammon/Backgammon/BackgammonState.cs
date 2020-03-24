using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    using static Defaults;
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public class BackgammonState
    {
        [JsonIgnore]
        public IDieRoller DieRoller { get; private set; }
        public Player? CurrentPlayer { get; private set; }
        public Player? Winner { get; private set; }
        public DiceState DiceRolls { get; private set; }
        public IReadOnlyList<PointState> Points { get; private set; } = Array.Empty<PointState>();
        public PointState Bar { get; private set; }
        public BackgammonState? Undo { get; private set; }

        public BackgammonState(IDieRoller dieRoller)
        {
            DieRoller = dieRoller;
        }

        public BackgammonState(BackgammonState original)
        {
            this.DieRoller = original.DieRoller;
            this.CurrentPlayer = original.CurrentPlayer;
            this.Winner = original.Winner;
            this.DiceRolls = original.DiceRolls;
            this.Points = original.Points;
            this.Bar = original.Bar;
        }

        public static BackgammonState DefaultState(IDieRoller dieRoller) =>
            new BackgammonState(dieRoller)
            {
                CurrentPlayer = null,
                Winner = null,
                DiceRolls = Defaults.EmptyDiceRolls,
                Points = StartingPosition,
                Bar = Defaults.EmptyPoint,
            };

        public BackgammonState With(
            BackgammonState? Undo,
            Player? CurrentPlayer = null,
            Player? Winner = null,
            DiceState? DiceRolls = null,
            IReadOnlyList<PointState>? Points = null,
            PointState? Bar = null
        ) {
            var points = Points ?? this.Points;
            System.Diagnostics.Debug.Assert(points.Count == 24);
            return new BackgammonState(this.DieRoller)
            {
                CurrentPlayer = CurrentPlayer ?? this.CurrentPlayer,
                Winner = Winner ?? this.Winner,
                DiceRolls = DiceRolls ?? this.DiceRolls,
                Points = Points ?? this.Points,
                Bar = Bar ?? this.Bar,
                Undo = Undo,
            };
        }  

    }

}