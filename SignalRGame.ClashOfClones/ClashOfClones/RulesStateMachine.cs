using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.ClashOfClones
{
    using ActionDispatcher = Func<ClashAction, Task<bool>>;

    public class RulesStateMachine
    {
        public static (ClashState, bool) ApplyAction(ClashState state, ClashAction? action)
        {
            if (action == null)
                return (state, false);
            switch (state)
            {
                case { IsReady: var isReady } when !isReady.White || !isReady.Black:
                    return action switch
                    {
                        ClashReady { Player: var player } when !isReady[player] => (state.With(IsReady: state.IsReady.With(player, true)), true),
                        _ => (state, false)
                    };
                case { Winner: null, CurrentPlayer: var currentPlayer }:
                    return action switch
                    {
                        ClashMove move when currentPlayer == move.Player  =>
                                 HandleMove(state, move),
                        _ => (state, false)
                    };
                case { Winner: Player winner }:
                    return action switch
                    {
                        ClashNewGame _ => (Defaults.DefaultState.With(CurrentPlayer: winner.OtherPlayer()), true),
                        _ => (state, false)
                    };
            }
        }

        private static (ClashState, bool) HandleMove(ClashState state, ClashMove move)
        {
            return (state, false);
        }

        public static async Task CheckAutomaticActions(ClashState state, ActionDispatcher dispatch)
        {
            await Task.Yield();
        }
    }
}
