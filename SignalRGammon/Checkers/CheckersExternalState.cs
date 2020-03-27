using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    public struct Move
    {
        public int CheckerIndex;
        public bool IsJump;
        public int[][] Moves;
    }

    public readonly struct CheckersExternalState
    {
        public CheckersState State { get; }
        public IReadOnlyList<Move>? ValidMovesForCurrentPlayer { get; }

        public CheckersExternalState(CheckersState state)
        {
            this.State = state;
            this.ValidMovesForCurrentPlayer = GetValidMoves(state);
        }

        public static IReadOnlyList<Move>? GetValidMoves(CheckersState state)
        {
            if (state.Winner != null)
                return null;
            var player = state.CurrentPlayer;
            var moves = (from tuple in state.Checkers[player].Select((checker, index) => (checker, index))
                    where tuple.checker != null
                    from move in GetValidMoves(tuple.checker!.Value, player, tuple.index, state.Checkers)
                    select move).ToArray();
            
            if (moves.Any(m => m.IsJump))
                return moves.Where(m => m.IsJump).ToArray();
            return moves;
        }

        private static readonly int[] bothDirections = new[] { -1, 1 };
        private static IEnumerable<Move> GetValidMoves(SingleChecker checker, Player player, int currentCheckerIndex, PlayerState<IReadOnlyList<SingleChecker?>> checkers)
        {
            return from rowOffset in ValidOffsets(checker, player)
                   from columnOffset in bothDirections
                   from move in (checker.Row + rowOffset, checker.Column + columnOffset) switch
                   {
                       (int row, int column)
                            when IsOpenSpace((row, column), player, currentCheckerIndex, checkers) =>
                                Of(new Move { IsJump = false, CheckerIndex = currentCheckerIndex, Moves = new[] { new[] { column, row } } }),
                       (int row, int column)
                            when HasCheckerAt(row, column, NonNullCheckers(checkers[player.OtherPlayer()])) 
                              && IsOpenSpace((row + rowOffset, column + columnOffset), player, currentCheckerIndex, checkers) => 
                                GetJumpMoves(checker.MoveTo(row + rowOffset, column + columnOffset, row == 0 || row == 7), player, currentCheckerIndex, checkers),
                       _ => Enumerable.Empty<Move>()
                   }
                   select move;

        }

        private static IEnumerable<Move> GetJumpMoves(SingleChecker checker, Player player, int currentCheckerIndex, PlayerState<IReadOnlyList<SingleChecker?>> checkers)
        {
            yield return new Move { IsJump = true, CheckerIndex = currentCheckerIndex, Moves = new[] { new[] { checker.Column, checker.Row } } };

            foreach (var move in GetValidMoves(checker, player, currentCheckerIndex, checkers))
            {
                if (move.IsJump)
                    yield return new Move { IsJump = true, CheckerIndex = currentCheckerIndex, Moves = new[] { new[] { checker.Column, checker.Row } }.Concat(move.Moves).ToArray() };
            }
        }

        private static bool IsOpenSpace((int row, int column) p, Player player, int currentCheckerIndex, PlayerState<IReadOnlyList<SingleChecker?>> checkers)
        {
            return p switch
            {
                (-1, _) => false,
                (8, _) => false,
                (_, -1) => false,
                (_, 8) => false,
                (int row, int column) => !HasCheckerAt(row, column, Except(checkers, player, currentCheckerIndex))
            };
        }

        private static bool HasCheckerAt(int row, int column, IEnumerable<SingleChecker> enumerable) =>
            (from checker in enumerable
             where checker.Row == row && checker.Column == column
             select checker).Any();

        private static IEnumerable<SingleChecker> Except(PlayerState<IReadOnlyList<SingleChecker?>> checkers, Player player, int currentCheckerIndex) =>
            NonNullCheckers(checkers[player].Select((c, i) => i == currentCheckerIndex ? null : c)
                .Concat(checkers[player.OtherPlayer()]));

        private static IEnumerable<SingleChecker> NonNullCheckers(IEnumerable<SingleChecker?> checkers) =>
            checkers
                .Where(c => c != null)
                .Select(c => c!.Value);

        private static IEnumerable<T> Of<T>(T value)
        {
            yield return value;
        }

        private static IEnumerable<int> ValidOffsets(SingleChecker checker, Player player)
        {
            if (!checker.IsKing)
                yield return player == Player.White ? 1 : -1;
            else
            {
                yield return -1;
                yield return 1;
            }
        }
    }
}
