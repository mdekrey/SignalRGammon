using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public enum Player
    {
        White,
        Black,
    }

    public readonly struct PlayerState<T>
    {
        public readonly T White;
        public readonly T Black;

        public PlayerState(T white, T black)
        {
            White = white;
            Black = black;
        }

        public PlayerState(Player player, T value, T otherValue) : this(otherValue, otherValue)
        {

            if (player == Player.White)
                White = value;
            else
                Black = value;
        }

        public T this[Player player] => player == Player.White ? White : Black;

        public PlayerState<T> With(Player player, T value) =>
            new PlayerState<T>(
                white: player == Player.White ? value : White,
                black: player == Player.Black ? value : Black
            );
    }


    public static class Defaults
    {
        public static DiceState EmptyDiceRolls = new DiceState(Array.Empty<int>(), Array.Empty<int>());
        public static PointState EmptyPoint = new PointState(0, 0);
    }

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

        private static IReadOnlyList<PointState> StartingPosition = new[]
        {
            new PointState(black: 2, white: 0),
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            new PointState(white: 5, black: 0),
            Defaults.EmptyPoint,
            new PointState(white: 3, black: 0),
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            new PointState(black: 5, white: 0),
            new PointState(white: 5, black: 0),
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            new PointState(black: 3, white: 0),
            Defaults.EmptyPoint,
            new PointState(black: 5, white: 0),
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            Defaults.EmptyPoint,
            new PointState(white: 2, black: 0),
        }.ToList().AsReadOnly();

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

        internal async Task<(BackgammonState, bool)> ApplyAction(BackgammonAction action)
        {
            await Task.Yield();
            switch (this)
            {
                case { CurrentPlayer: null, DiceRolls: { White: { Count: 1 }, Black: { Count: 1 } } }:
                    return action switch
                    {
                        BackgammonSetStartingPlayer { Player: null } => (DefaultState(DieRoller), true),
                        BackgammonSetStartingPlayer { Player: Player player } => (this.With(CurrentPlayer: player, DiceRolls: Defaults.EmptyDiceRolls.With(player, new[] { DiceRolls.White[0], DiceRolls.Black[0] })), true),
                        _ => (this, false)
                    };
                case { CurrentPlayer: null }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var player } when DiceRolls[player].Count == 0 => (this.With(DiceRolls: DiceRolls.With(player, new[] { DieRoller.RollDie() })), true),
                        _ => (this, false)
                    };
                case { CurrentPlayer: Player currentPlayer }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var actingPlayer } when actingPlayer == currentPlayer && DiceRolls[currentPlayer].Count == 0 => (this.With(DiceRolls: Defaults.EmptyDiceRolls.With(currentPlayer, RollDiceWithDoubles())), true),
                        _ => (this, false)
                    };
            }
        }

        private IReadOnlyList<int> RollDiceWithDoubles()
        {
            return (DieRoller.RollDie(6), DieRoller.RollDie(6)) switch
            {
                (int a, int b) when a == b => Enumerable.Repeat(a, 4).ToArray(),
                (int a, int b) => new[] { a, b },
            };
        }
    }
}