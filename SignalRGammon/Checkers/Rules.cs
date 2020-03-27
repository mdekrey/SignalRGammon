using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    using ActionDispatcher = Func<CheckersAction, Task<bool>>;

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
                        CheckersReady { Player: var player } when !isReady[player] => (state.With(IsReady: state.IsReady.With(player, true)), true),
                        _ => (state, false)
                    };
                case { Winner: null, CurrentPlayer: var currentPlayer, Checkers: var checkers }:
                    return action switch
                    {
                        // TODO - move
                        CheckersDeclareWinner { Player: var winner } => (state.With(Winner: winner), true),
                        _ => (state, false)
                    };
                case { Winner: Player winner }:
                    return action switch
                    {
                        CheckersNewGame _ => (state.With(CurrentPlayer: winner.OtherPlayer(), Winner: null, Checkers: Defaults.InitialCheckers), true),
                        _ => (state, false)
                    };
            }
        }

        public static async Task CheckAutomaticActions(CheckersState state, ActionDispatcher dispatch)
        {
            await Task.Yield();
        }
    }
}
