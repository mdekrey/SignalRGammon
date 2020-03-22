using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    using static BackgammonState;
    using PointState = PlayerState<int>;
    using DiceState = PlayerState<IReadOnlyList<int>>;


    public static class Rules
    {
        public static async Task CheckAutomaticActions(BackgammonState obj, Func<BackgammonAction, Task<bool>> Do)
        {
            switch (obj)
            {
                case { CurrentPlayer: null, DiceRolls: { White: { Count: 1 }, Black: { Count: 1 } } }:
                    await Do(new BackgammonSetStartingPlayer());
                    break;
            }
        }

        public static async Task<(BackgammonState, bool)> ApplyAction(this BackgammonState state, BackgammonAction action)
        {
            await Task.Yield();
            switch (state)
            {
                case { CurrentPlayer: null, DiceRolls: { White: { Count: 1 }, Black: { Count: 1 } } }:
                    if (!(action is BackgammonSetStartingPlayer))
                        return (state, false);
                    return (state.DiceRolls.White[0], state.DiceRolls.Black[0]) switch
                    {
                        (int white, int black) when white < black => (state.With(CurrentPlayer: Player.Black, DiceRolls: Defaults.EmptyDiceRolls.With(Player.Black, new[] { black, white })), true),
                        (int white, int black) when white > black => (state.With(CurrentPlayer: Player.White, DiceRolls: Defaults.EmptyDiceRolls.With(Player.White, new[] { white, black })), true),
                        _ => (DefaultState(state.DieRoller), true),
                    };
                case { CurrentPlayer: null }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var player } when state.DiceRolls[player].Count == 0 => (state.With(DiceRolls: state.DiceRolls.With(player, new[] { state.DieRoller.RollDie() })), true),
                        _ => (state, false)
                    };
                case { CurrentPlayer: Player currentPlayer }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var actingPlayer } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Count == 0 => (state.With(DiceRolls: Defaults.EmptyDiceRolls.With(currentPlayer, state.DieRoller.RollDiceWithDoubles())), true),
                        BackgammonMove { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Contains(dieValue) =>
                            state.HandleMove(dieValue, startingPoint),
                        BackgammonBearOff { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Contains(dieValue) =>
                            state.HandleBearOff(dieValue, startingPoint),
                        BackgammonCannotUseRoll { DieValue: var dieValue } =>
                            state.RevokeDieRoll(dieValue),
                        _ => (state, false)
                    };
            }
        }


        private static (BackgammonState, bool) HandleMove(this BackgammonState state, int dieValue, int startingPoint)
        {
            if (!state.CurrentPlayer.HasValue)
                // Can only make a move on a player's turn
                return (state, false);

            var player = state.CurrentPlayer.Value;
            var otherPlayer = player.OtherPlayer();
            if (startingPoint != -1 && state.Bar[player] != 0)
                // Any time a player has one or more checkers on the bar, his first obligation is to enter those checker(s) into the opposing home board.
                return (state, false);
            if (startingPoint == -1 && state.Bar[player] <= 0)
                // Player has nothing on the bar, but is trying to move off the bar
                return (state, false);

            var actualEndPoint = GetEndPoint(player, startingPoint, dieValue);

            if (state.IsAnchor(actualEndPoint, otherPlayer))
            {
                // tried to move onto the other players' anchor
                return (state, false);
            }

            var points = state.Points.ToImmutableList();

            var bar = state.Points[actualEndPoint][otherPlayer] == 1
                ? state.Bar.With(otherPlayer, state.Bar[otherPlayer] + 1) // hit a blot
                : state.Bar;

            points = points.SetItem(actualEndPoint, new PointState(player, state.Points[actualEndPoint][player] + 1, 0));

            if (startingPoint < 0)
            {
                bar = bar.With(player, bar[player] - 1);
            }
            else
            {
                points = points.SetItem(startingPoint, new PointState(player, state.Points[startingPoint][player] - 1, 0));
            }

            var resultDice = state.DiceRolls[player].ToList();
            resultDice.RemoveAt(resultDice.IndexOf(dieValue));
            return (
                state.With(
                    CurrentPlayer: resultDice.Count == 0 ? player.OtherPlayer() : player,
                    DiceRolls: state.DiceRolls.With(player, resultDice.AsReadOnly()),
                    Points: points,
                    Bar: bar
                ),
                true
            );
        }

        private static (BackgammonState, bool) HandleBearOff(this BackgammonState state, int dieValue, int startingPoint)
        {
            if (!state.CurrentPlayer.HasValue)
                // Can only bear off on a player's turn
                return (state, false);

            var player = state.CurrentPlayer.Value;
            if (state.Bar[player] != 0 || !(from point in Enumerable.Range(0, 24)
                                      where GetEffectiveStartPoint(player, point) < 18
                                      select state.Points[point][player] == 0).Any())
                // All checkers must be on the players' home board to bear off
                return (state, false);

            var effectiveEndPoint = GetEffectiveEndPoint(player, startingPoint, dieValue);
            if (effectiveEndPoint < 24)
                // Checker is not close enough to bear off
                return (state, false);

            var points = state.Points.ToImmutableList()
                .SetItem(startingPoint, new PointState(player, state.Points[startingPoint][player] - 1, 0));

            var resultDice = state.DiceRolls[player].ToList();
            resultDice.RemoveAt(resultDice.IndexOf(dieValue));
            return (
                state.With(
                    CurrentPlayer: resultDice.Count == 0 ? player.OtherPlayer() : player,
                    DiceRolls: state.DiceRolls.With(player, resultDice.AsReadOnly()),
                    Points: points
                ),
                true
            );
        }

        private static (BackgammonState, bool) RevokeDieRoll(this BackgammonState state, int dieValue)
        {
            if (!state.CurrentPlayer.HasValue)
                // Can only revoke a roll on a player's turn
                return (state, false);

            var player = state.CurrentPlayer.Value;
            var resultDice = state.DiceRolls[player].ToList();
            resultDice.RemoveAt(resultDice.IndexOf(dieValue));
            return (
                state.With(
                    DiceRolls: state.DiceRolls.With(player, resultDice.AsReadOnly())
                ),
                true
            );
        }

        public static int GetEndPoint(Player player, int startingPoint, int dieValue)
        {
            var effectiveEndPoint = GetEffectiveEndPoint(player, startingPoint, dieValue);
            var actualEndPoint = player == Player.White ? 23 - effectiveEndPoint
                : effectiveEndPoint;
            return actualEndPoint;
        }

        private static int GetEffectiveEndPoint(Player player, int startingPoint, int dieValue)
        {
            var effectiveStartPoint = GetEffectiveStartPoint(player, startingPoint);
            return effectiveStartPoint + dieValue;
        }

        private static int GetEffectiveStartPoint(Player player, int startingPoint)
        {
            return startingPoint == -1 ? -1
                   : player == Player.White ? 23 - startingPoint
                   : startingPoint;
        }

        public static bool IsAnchor(this BackgammonState state, int actualEndPoint, Player player)
        {
            return state.Points[actualEndPoint][player] > 1;
        }

        private static IReadOnlyList<int> RollDiceWithDoubles(this IDieRoller dieRoller)
        {
            return (dieRoller.RollDie(6), dieRoller.RollDie(6)) switch
            {
                (int a, int b) when a == b => Enumerable.Repeat(a, 4).ToArray(),
                (int a, int b) => new[] { a, b },
            };
        }
    }
}
