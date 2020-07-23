using System.Security.Claims;

namespace SignalRGame.GameUtilities
{
    public interface IGameLogic<TPersistedState, TPublicState, TAction>
    {
        TPersistedState InitialState();
        (TPersistedState newState, bool isValid) PerformAction(TPersistedState state, TAction action, ClaimsPrincipal? user);
        (TAction action, bool hasAction) GetRecommendedAction(TPersistedState state, ClaimsPrincipal? user);
        TPublicState ToPublicGameState(TPersistedState state, ClaimsPrincipal? user);
    }
}