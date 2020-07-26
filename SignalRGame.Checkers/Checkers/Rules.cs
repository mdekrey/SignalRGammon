using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Checkers
{
    public class Rules
    {
        public static (CheckersState, bool) ApplyAction(CheckersState state, CheckersAction? action)
        {
            if (action == null)
                return (state, false);
            switch (state)
            {
                case { IsReady: var isReady } when !isReady.White || !isReady.Black:
                    return action switch
                    {
                        CheckersReady { Player: var player } when !isReady[player] => (state with { IsReady = state.IsReady.With(player, true) }, true),
                        _ => (state, false)
                    };
                case { Winner: null, CurrentPlayer: var currentPlayer, MovingChecker: var checkerIndex }:
                    return action switch
                    {
                        CheckersMove move when currentPlayer == move.Player  =>
                                 HandleMove(state, move),
                        CheckersCannotMove _ when checkerIndex != null => (state with { CurrentPlayer = currentPlayer.OtherPlayer(), MovingChecker = null }, true),
                        CheckersCannotMove _ when checkerIndex == null => (state with { Winner = currentPlayer.OtherPlayer() }, true),
                        _ => (state, false)
                    };
                case { Winner: Player winner }:
                    return action switch
                    {
                        CheckersNewGame _ => (new CheckersState { CurrentPlayer = winner.OtherPlayer() }, true),
                        _ => (state, false)
                    };
                default:
                    return (state, false);
            }
        }

        private static (CheckersState, bool) HandleMove(CheckersState state, CheckersMove move)
        {
            switch ((from m in CheckersExternalState.GetValidMoves(state)
                    where m.CheckerIndex == move.PieceIndex && m.Column == move.Column && m.Row == move.Row
                    select (Move?)m).FirstOrDefault())
            {
                case Move { IsJump: true }:
                    {
                        var checker = state.Checkers[state.CurrentPlayer][move.PieceIndex]!;
                        var jumped = (column: (checker.Column + move.Column) / 2, row: (checker.Row + move.Row) / 2);
                        var player = state.CurrentPlayer;
                        var other = player.OtherPlayer();
                        return (
                            state with
                            {
                                MovingChecker = move.PieceIndex,
                                Checkers = state.Checkers
                                        .With(player, state.Checkers[player].ToImmutableList().SetItem(move.PieceIndex, checker.MoveTo(move.Column, move.Row)))
                                        .With(other, state.Checkers[other].Select(c => c == null || (c.Column == jumped.column && c.Row == jumped.row) ? null : c).ToArray())
                            },
                            true
                        );
                    }
                case Move { IsJump: false }:
                    {
                        var checker = state.Checkers[state.CurrentPlayer][move.PieceIndex]!;
                        return (
                            state with {
                                CurrentPlayer = state.CurrentPlayer.OtherPlayer(),
                                MovingChecker = null,
                                Checkers = state.Checkers.With(state.CurrentPlayer, state.Checkers[state.CurrentPlayer].ToImmutableList().SetItem(move.PieceIndex, checker.MoveTo(move.Column, move.Row)))
                            },
                            true
                        );
                    }
                case null:
                    return (state, false);
            }
        }

        public static (CheckersAction? action, bool hasAction) GetAutomaticAction(CheckersState state)
        {
            switch (state)
            {
                case { Winner: null } when !CheckersExternalState.GetValidMoves(state).Any():
                    return (new CheckersCannotMove(), true);
                default:
                    return (null, false);
            }
        }
    }
}
