using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    public enum Player
    {
        White,
        Black,
    }


    public readonly struct PointState
    {
        public readonly int White;
        public readonly int Black;

        public PointState(int white = 0, int black = 0)
        {
            White = white;
            Black = black;
        }

        public PointState(Player player, int count) : this(0, 0)
        {
            if (player == Player.White)
                White = count;
            else
                Black = count;
        }

        public int this[Player player] => player == Player.White ? White : Black;

        public PointState With(Player player, int count) =>
            new PointState(
                white: player == Player.White ? count : White,
                black: player == Player.Black ? count : Black
            );

        public static PointState Empty = new PointState();
    }

    public struct BackgammonState
    {
        [JsonIgnore]
        public IDieRoller DieRoller { get; private set; }
        public Player? CurrentPlayer { get; private set; }
        public IReadOnlyList<int> WhiteDiceRolls { get; private set; }
        public IReadOnlyList<int> BlackDiceRolls { get; private set; }
        public IReadOnlyList<PointState> Points { get; private set; }
        public PointState Fence { get; private set; }

        public BackgammonState(IDieRoller dieRoller) : this()
        {
            DieRoller = dieRoller;
        }

        public BackgammonState(BackgammonState original)
        {
            this.DieRoller = original.DieRoller;
            this.CurrentPlayer = original.CurrentPlayer;
            this.WhiteDiceRolls = original.WhiteDiceRolls;
            this.BlackDiceRolls = original.BlackDiceRolls;
            this.Points = original.Points;
            this.Fence = original.Fence;
        }

        private static IReadOnlyList<PointState> StartingPosition = new[]
        {
            new PointState(white: 5),
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            new PointState(black: 3),
            PointState.Empty,
            new PointState(black: 5),
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            new PointState(white: 2),
            new PointState(black: 2),
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            new PointState(white: 5),
            PointState.Empty,
            new PointState(white: 3),
            PointState.Empty,
            PointState.Empty,
            PointState.Empty,
            new PointState(black: 5),
        }.ToList().AsReadOnly();

        public static BackgammonState DefaultState(IDieRoller dieRoller) =>
            new BackgammonState
            {
                DieRoller = dieRoller,
                CurrentPlayer = null,
                WhiteDiceRolls = Array.Empty<int>(),
                BlackDiceRolls = Array.Empty<int>(),
                Points = StartingPosition,
                Fence = PointState.Empty,
            };

        internal async Task<(BackgammonState, bool)> ApplyAction(BackgammonAction action)
        {
            await Task.Yield();
            switch (CurrentPlayer)
            {
                case null:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: Player.White } when WhiteDiceRolls.Count == 0 && BlackDiceRolls.Count == 0 => (new BackgammonState(this) { WhiteDiceRolls = new[] { DieRoller.RollDie() } }, true),
                        BackgammonDiceRoll { Player: Player.Black } when BlackDiceRolls.Count == 0 && WhiteDiceRolls.Count == 0 => (new BackgammonState(this) { BlackDiceRolls = new[] { DieRoller.RollDie() } }, true),
                        BackgammonDiceRoll { Player: Player.White } when WhiteDiceRolls.Count == 0 => (DetermineStartingPlayer(new BackgammonState(this) { WhiteDiceRolls = new[] { DieRoller.RollDie() } }), true),
                        BackgammonDiceRoll { Player: Player.Black } when BlackDiceRolls.Count == 0 => (DetermineStartingPlayer(new BackgammonState(this) { BlackDiceRolls = new[] { DieRoller.RollDie() } }), true),
                        _ => (this, false)
                    };
                default:
                    return (this, false);
            }
        }

        private static BackgammonState DetermineStartingPlayer(BackgammonState backgammonState)
        {
            return (backgammonState.WhiteDiceRolls[0], backgammonState.BlackDiceRolls[0]) switch
            {
                (var white, var black) when white > black => new BackgammonState(backgammonState) { CurrentPlayer = Player.White, WhiteDiceRolls = new[] { white, black }, BlackDiceRolls = Array.Empty<int>() },
                (var white, var black) when white < black => new BackgammonState(backgammonState) { CurrentPlayer = Player.Black, BlackDiceRolls = new[] { black, white }, WhiteDiceRolls = Array.Empty<int>() },
                
                // equal
                _ => DefaultState(backgammonState.DieRoller),
            };
        }
    }
}