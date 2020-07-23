using Newtonsoft.Json;
using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalRGame.Checkers
{
    public class CheckersGame : IGameLogic<CheckersState, CheckersExternalState, CheckersAction?>
    {
        public (CheckersAction? action, bool hasAction) GetRecommendedAction(CheckersState state, ClaimsPrincipal? user) => Rules.GetAutomaticAction(state);

        public CheckersState InitialState() => Defaults.DefaultState;

        public (CheckersState newState, bool isValid) PerformAction(CheckersState state, CheckersAction? action, ClaimsPrincipal? user) => Rules.ApplyAction(state, action);

        public CheckersExternalState ToPublicGameState(CheckersState state, ClaimsPrincipal? user) => new CheckersExternalState(state);
    }
}
