﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Backgammon
{
    using static Enumerable;
    using static BackgammonState;
    using PointState = PlayerState<int>;
    using ActionDispatcher = Func<BackgammonAction, Task<bool>>;

    public static class Rules
    {
        public static async Task CheckAutomaticActions(BackgammonState state, ActionDispatcher dispatch)
        {
            try
            {
                switch (state)
                {
                    case { Winner: null, CurrentPlayer: null, DiceRolls: { White: { Count: 1 }, Black: { Count: 1 } } }:
                        await dispatch(new BackgammonSetStartingPlayer());
                        break;
                    case { Winner: null, DiceRolls: { White: { Count: 0 }, Black: { Count: 0 } } }:
                        await CheckForWinner(state, dispatch);
                        break;
                    case { Winner: null, CurrentPlayer: Player player, DiceRolls: var diceRolls } when diceRolls[player].Count > 0:
                        await DoInvalidDieRolls(state, dispatch);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task CheckForWinner(BackgammonState state, ActionDispatcher dispatch)
        {
            if (state.Points.All(p => p.White == 0))
            {
                await dispatch(new BackgammonDeclareWinner { Player = Player.White });
            }
            else if (state.Points.All(p => p.Black == 0))
            {
                await dispatch(new BackgammonDeclareWinner { Player = Player.Black });
            }
        }

        private static async Task DoInvalidDieRolls(BackgammonState state, ActionDispatcher dispatch)
        {
            if (!(state.CurrentPlayer is Player player))
                return;

            var orderedDice = state.DiceRolls[player].OrderBy(v => v).ToArray();
            var validFirstStates = (from die in orderedDice
                                   from nextState in ValidStates(state, die)
                                   group nextState by die).ToArray();
            if (validFirstStates.All(e => e.Count() == 0))
            {
                // no valid moves
                await dispatch(new BackgammonCannotUseRoll { DieValues = orderedDice });
                return;
            }

            // We can't really remove other dice due to it maybe becoming valid in a different order; we'd have to disable dice in the first round, which isn't exactly something currently in the state machine...
        }

        static IEnumerable<BackgammonState> ValidStates(BackgammonState obj, int dieRoll)
        {
            var validStartingPoints = (from idx in Range(-1, 25)
                                       from tuple in new (BackgammonAction action, int idx)[]
                                       {
                                           (new BackgammonMove { DieValue = dieRoll, Player = obj.CurrentPlayer.Value, StartingPointNumber = idx }, idx),
                                           (new BackgammonBearOff { DieValue = dieRoll, Player = obj.CurrentPlayer.Value, StartingPointNumber = idx }, idx),
                                       }
                                       let applied = obj.ApplyAction(tuple.action)
                                       where applied.Item2
                                       select applied.Item1).ToArray();
            return validStartingPoints;
        }

        public static (BackgammonState, bool) ApplyAction(this BackgammonState state, BackgammonAction action)
        {
            switch (state)
            {
                case { Winner: null, CurrentPlayer: null, DiceRolls: { White: { Count: 1 }, Black: { Count: 1 } } }:
                    if (!(action is BackgammonSetStartingPlayer))
                        return (state, false);
                    return (state.DiceRolls.White[0], state.DiceRolls.Black[0]) switch
                    {
                        (int white, int black) when white < black => (state.With(CurrentPlayer: Player.Black, DiceRolls: Defaults.EmptyDiceRolls.With(Player.Black, new[] { black, white })), true),
                        (int white, int black) when white > black => (state.With(CurrentPlayer: Player.White, DiceRolls: Defaults.EmptyDiceRolls.With(Player.White, new[] { white, black })), true),
                        _ => (DefaultState(state.DieRoller), true),
                    };
                case { Winner: null, CurrentPlayer: null }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var player } when state.DiceRolls[player].Count == 0 => (state.With(DiceRolls: state.DiceRolls.With(player, new[] { state.DieRoller.RollDie() })), true),
                        _ => (state, false)
                    };
                case { Winner: null, CurrentPlayer: Player currentPlayer }:
                    return action switch
                    {
                        BackgammonDiceRoll { Player: var actingPlayer } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Count == 0 => (state.With(DiceRolls: Defaults.EmptyDiceRolls.With(currentPlayer, state.DieRoller.RollDiceWithDoubles())), true),
                        BackgammonMove { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Contains(dieValue) =>
                            state.HandleMove(dieValue, startingPoint),
                        BackgammonBearOff { Player: var actingPlayer, DieValue: var dieValue, StartingPointNumber: var startingPoint } when actingPlayer == currentPlayer && state.DiceRolls[currentPlayer].Contains(dieValue) =>
                            state.HandleBearOff(dieValue, startingPoint),
                        BackgammonCannotUseRoll { DieValues: var dieValues } =>
                            state.RevokeDieRoll(dieValues),
                        BackgammonDeclareWinner { Player: var player } =>
                            state.DeclareWinner(player),
                        _ => (state, false)
                    };
                case { Winner: Player _ }:
                    return action switch
                    {
                        BackgammonNewGame _ => (DefaultState(state.DieRoller), true),
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
            if (startingPoint != -1 && state.Points[startingPoint][player] <= 0)
                // Player has nothing at this point
                return (state, false);

            var actualEndPoint = GetEndPoint(player, startingPoint, dieValue);

            if (actualEndPoint >= 24)
                // Used "move" to bear off.
                return (state, false);

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
            if (startingPoint == -1)
                // not possible to bear off from the bar
                return (state, false);

            var player = state.CurrentPlayer.Value;
            if (state.Points[startingPoint][player] <= 0)
                // Player has nothing at this point
                return (state, false);

            if (state.Bar[player] != 0 || !(from point in Range(0, 24)
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

        private static (BackgammonState, bool) RevokeDieRoll(this BackgammonState state, IEnumerable<int> dieValues)
        {
            if (!state.CurrentPlayer.HasValue)
                // Can only revoke a roll on a player's turn
                return (state, false);

            var player = state.CurrentPlayer.Value;
            var resultDice = state.DiceRolls[player].ToList();
            foreach (var dieValue in dieValues)
            {
                resultDice.RemoveAt(resultDice.IndexOf(dieValue));
            }
            return (
                state.With(
                    CurrentPlayer: resultDice.Count == 0 ? player.OtherPlayer() : player,
                    DiceRolls: state.DiceRolls.With(player, resultDice.AsReadOnly())
                ),
                true
            );
        }

        private static (BackgammonState, bool) DeclareWinner(this BackgammonState state, Player player)
        {
            if (!state.Points.All(p => p[player] == 0))
                // Can only revoke a roll on a player's turn
                return (state, false);

            return (
                state.With(
                    Winner: player
                ),
                true
            );
        }

        public static int GetEndPoint(Player player, int startingPoint, int dieValue)
        {
            var effectiveEndPoint = GetEffectiveEndPoint(player, startingPoint, dieValue);
            var actualEndPoint = effectiveEndPoint >= 24 ? 24 // not an end-point, but bearing-off
                : player == Player.White ? 23 - effectiveEndPoint
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
            System.Diagnostics.Debug.Assert(actualEndPoint >= 0);
            System.Diagnostics.Debug.Assert(actualEndPoint < 24);
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
