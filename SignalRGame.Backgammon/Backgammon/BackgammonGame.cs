using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SignalRGame.GameUtilities;

namespace SignalRGame.Backgammon
{

    public class BackgammonGame : IGameLogic<BackgammonState, BackgammonState, BackgammonAction?>
    {
        private readonly Rules rules;

        public BackgammonGame(IDieRoller dieRoller)
        {
            rules = new Rules(dieRoller);
        }

        public BackgammonState InitialState() => BackgammonState.DefaultState();

        public (BackgammonState newState, bool isValid) PerformAction(BackgammonState state, BackgammonAction? action, ClaimsPrincipal? user) => rules.ApplyAction(state, action);

        public (BackgammonAction? action, bool hasAction) GetRecommendedAction(BackgammonState state, ClaimsPrincipal? user) => rules.GetAutomaticActions(state);

        public BackgammonState ToPublicGameState(BackgammonState state, ClaimsPrincipal? user) => state;
    }
}
