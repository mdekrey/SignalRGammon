﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SignalRGame.Backgammon
{
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;

    public class RulesShould
    {
        public static PointState Black(int count) => new PointState(black: count, white: 0);
        public static PointState White(int count) => new PointState(black: 0, white: count);
        public static PointState NoCheckers() => new PointState(black: 0, white: 0);

        [Fact]
        public async Task CatchWhenNoMovesCanBeMadeByBlack()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(4, a),
                        a => Assert.Equal(5, a)
                    );
                }
            );
        }

        [Fact]
        public async Task CatchWhenCannotMoveOffBarByBlack()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 6 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(white: 0, black: 1)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(6, a)
                    );
                }
            );
        }

        [Fact]
        public async Task AllowValidMovesByBlack()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Empty(actions);
        }

        [Fact]
        public async Task AllowValidMovesByBlackInOrder()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        Black(1),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Empty(actions);
        }

        [Fact(Skip = "We can't really remove other dice due to it maybe becoming valid in a different order; we'd have to disable dice in the first round, which isn't exactly something currently in the state machine...")]
        public async Task CatchWhenOnlyOneMoveCanBeMadeByBlack()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        Black(1),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(4, a)
                    );
                }
            );
        }

        [Fact]
        public async Task RemovesDiceWhenBlackWins()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.Black,
                    DiceRolls: new DiceState(Player.Black, new[] { 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(5, a)
                    );
                }
            );
        }

        [Fact]
        public async Task CatchWhenNoMovesCanBeMadeByWhite()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(4, a),
                        a => Assert.Equal(5, a)
                    );
                }
            );
        }

        [Fact]
        public async Task CatchWhenCannotMoveOffBarByWhite()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 6 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(2),
                    },
                    Bar: new PointState(white: 1, black: 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(6, a)
                    );
                }
            );
        }

        [Fact]
        public async Task AllowValidMovesByWhite()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        White(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Empty(actions);
        }

        [Fact]
        public async Task AllowValidMovesByWhiteInOrder()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(1),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Empty(actions);
        }

        [Fact(Skip = "We can't really remove other dice due to it maybe becoming valid in a different order; we'd have to disable dice in the first round, which isn't exactly something currently in the state machine...")]
        public async Task CatchWhenOnlyOneMoveCanBeMadeByWhite()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 4, 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        White(1),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(4, a)
                    );
                }
            );
        }

        [Fact]
        public async Task RemovesDiceWhenWhiteWins()
        {
            var rules = new Rules(new FakeDieRoller());
            var state = BackgammonState.DefaultState()
                .With(
                    null,
                    CurrentPlayer: Player.White,
                    DiceRolls: new DiceState(Player.White, new[] { 5 }, Array.Empty<int>()),
                    Points: new[]
                    {
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        Black(2),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                        NoCheckers(),
                    },
                    Bar: new PointState(0, 0)
                );

            var actions = new List<BackgammonAction>();
            Task<bool> ExpectedDispatch(BackgammonAction arg)
            {
                actions.Add(arg);
                return Task.FromResult(true);
            }
            await rules.CheckAutomaticActions(state, ExpectedDispatch);

            Assert.Collection(
                actions,
                invalid =>
                {
                    var rolls = Assert.IsType<BackgammonCannotUseRoll>(invalid);
                    Assert.Collection(rolls.DieValues,
                        a => Assert.Equal(5, a)
                    );
                }
            );
        }

    }
}