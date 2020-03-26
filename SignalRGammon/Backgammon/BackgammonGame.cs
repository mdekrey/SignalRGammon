using System.Collections.Generic;
using System.Threading.Tasks;
using SignalRGammon.GameUtilities;

namespace SignalRGammon.Backgammon
{

    public class BackgammonGame : GameBase<BackgammonState, BackgammonState, BackgammonAction?>
    {
        private readonly Rules rules;

        public BackgammonGame(IDieRoller dieRoller) : base(BackgammonState.DefaultState())
        {
            rules = new Rules(dieRoller);
        }

        protected override BackgammonState GetExternalState(BackgammonState state) => state;
        protected override Task<(BackgammonState newState, bool isValid)> ApplyAction(BackgammonState state, BackgammonAction? action) =>
            Task.FromResult(rules.ApplyAction(state, action));
        protected override Task CheckAutomaticActions(BackgammonState state) =>
            rules?.CheckAutomaticActions(state, Do) ?? Task.CompletedTask;
    }
}
