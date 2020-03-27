using SignalRGammon.GameUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGammon.Checkers
{
    public class CheckersGame : GameBase<CheckersState, CheckersState, CheckersAction?>
    {
        public CheckersGame() : base(Defaults.DefaultState)
        {
        }

        protected override CheckersState GetExternalState(CheckersState state) => state;
        protected override Task<(CheckersState newState, bool isValid)> ApplyAction(CheckersState state, CheckersAction? action) =>
            Task.FromResult(Rules.ApplyAction(state, action));
        protected override Task CheckAutomaticActions(CheckersState state) =>
            Rules.CheckAutomaticActions(state, Do) ?? Task.CompletedTask;
    }
}
