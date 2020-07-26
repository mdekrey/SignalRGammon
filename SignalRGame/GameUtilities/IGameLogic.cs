using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SignalRGame.GameUtilities
{
    public interface IGameLogic<TPersistedState, TPublicState, TAction> : IGameLogic
    {
        new TPersistedState InitialState();
        (TPersistedState newState, bool isValid) PerformAction(TPersistedState state, TAction action, ClaimsPrincipal? user);
        (TAction action, bool hasAction) GetRecommendedAction(TPersistedState state, ClaimsPrincipal? user);
        TPublicState ToPublicGameState(TPersistedState state, [MaybeNull] TAction action, ClaimsPrincipal? user);

        string FromState(TPersistedState state);
        new TPersistedState ToState(string state);
        string FromAction(TAction action);
        new TAction ToAction(string action);
        string FromPublicState(TPublicState action);


        string IGameLogic.FromState(GameState state) => FromState((TPersistedState)state.State!);
        GameState IGameLogic.ToState(string state) => new GameState(ToState(state));
        string IGameLogic.FromAction(GameAction action) => FromAction((TAction)action.Action!);
        GameAction IGameLogic.ToAction(string action) => new GameAction(ToAction(action));
        GameState IGameLogic.InitialState() => new GameState(InitialState());
        (GameState newState, bool isValid) IGameLogic.PerformAction(GameState state, GameAction action, ClaimsPrincipal? user) => 
            PerformAction((TPersistedState)state.State!, (TAction)action.Action!, user) is var (s, isValid) 
            ? (new GameState(s), isValid)
            : throw new InvalidOperationException();
        (GameAction action, bool hasAction) IGameLogic.GetRecommendedAction(GameState state, ClaimsPrincipal? user) => 
            GetRecommendedAction((TPersistedState)state.State!, user) is var (a, hasAction)
            ? (new GameAction(a), hasAction)
            : throw new InvalidOperationException();
        string IGameLogic.ToPublicGameState(GameState state, GameAction? action, ClaimsPrincipal? user) => FromPublicState(ToPublicGameState((TPersistedState)state.State!, (TAction)action?.Action!, user));
    }

    public record GameState(object? State);
    public record GameAction(object? Action);

    public interface IGameLogic
    {
        string FromState(GameState state);
        GameState ToState(string state);
        string FromAction(GameAction action);
        GameAction ToAction(string action);


        GameState InitialState();
        (GameState newState, bool isValid) PerformAction(GameState state, GameAction action, ClaimsPrincipal? user);
        (GameAction action, bool hasAction) GetRecommendedAction(GameState state, ClaimsPrincipal? user);
        string ToPublicGameState(GameState state, GameAction? action, ClaimsPrincipal? user);
    }
}