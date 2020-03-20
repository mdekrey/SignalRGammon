using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    public static class PlayerExtensions
    {
        public static Player OtherPlayer(this Player p)
        {
            return p == Player.White ? Player.Black : Player.White;
        }
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
                        BackgammonMove { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && DiceRolls[currentPlayer].Contains(dieValue) => 
                            HandleMove(dieValue, startingPoint),
                        //BackgammonBearOff { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && DiceRolls[currentPlayer].Contains(dieValue) =>
                        //    HandleBearOff(dieValue, startingPoint),
                        _ => (this, false)
                    };
            }
        }

        private (BackgammonState, bool) HandleMove(int dieValue, int startingPoint)
        {
            if (!CurrentPlayer.HasValue)
                // Can only make a move on a player's turn
                return (this, false);

            var player = CurrentPlayer.Value;
            var otherPlayer = player.OtherPlayer();
            if (startingPoint != -1 && Bar[player] != 0)
                // Any time a player has one or more checkers on the bar, his first obligation is to enter those checker(s) into the opposing home board.
                return (this, false);
            if (startingPoint == -1 && Bar[player] <= 0)
                // Player has nothing on the bar, but is trying to move off the bar
                return (this, false);

            var actualEndPoint = GetEndPoint(player, startingPoint, dieValue);

            if (IsAnchor(actualEndPoint, otherPlayer))
            {
                // tried to move onto the other players' anchor
                return (this, false);
            }

            var points = Points.ToImmutableList();

            var bar = Points[actualEndPoint][otherPlayer] == 1
                ? Bar.With(otherPlayer, Bar[otherPlayer] + 1) // hit a blot
                : Bar;

            points = points.SetItem(actualEndPoint, new PointState(player, Points[actualEndPoint][player] + 1, 0));

            if (startingPoint < 0)
            {
                bar = bar.With(player, bar[player] - 1);
            }
            else
            {
                points = points.SetItem(startingPoint, new PointState(player, Points[startingPoint][player] - 1, 0));
            }

            var resultDice = DiceRolls[player].ToList();
            resultDice.RemoveAt(resultDice.IndexOf(dieValue));
            return (
                this.With(
                    CurrentPlayer: resultDice.Count == 0 ? otherPlayer : player,
                    DiceRolls: DiceRolls.With(player, resultDice.AsReadOnly()),
                    Points: points,
                    Bar: bar
                ),
                true
            );
        }

        public int GetEndPoint(Player player, int startingPoint, int dieValue)
        {

            var effectiveStartPoint =
                startingPoint == -1 ? -1
                : player == Player.White ? 23 - startingPoint
                : startingPoint;
            var effectiveEndPoint = effectiveStartPoint + dieValue;
            var actualEndPoint = player == Player.White ? 23 - effectiveEndPoint
                : effectiveEndPoint;
            return actualEndPoint;
        }

        public bool IsAnchor(int actualEndPoint, Player player)
        {
            return Points[actualEndPoint][player] > 1;
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