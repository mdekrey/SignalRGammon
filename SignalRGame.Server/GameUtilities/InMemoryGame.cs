using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;


namespace SignalRGame.GameUtilities
{
    public sealed class InMemoryGame<TExternalState, TInternalState, TAction> : IGame<TExternalState, TAction>
    {
        private readonly IGameLogic<TInternalState, TExternalState, TAction> gameLogic;
        private readonly BehaviorSubject<(TInternalState state, TAction action)> _state;
        private readonly IObservable<(TInternalState state, TAction action)> states;

        public InMemoryGame(IGameLogic<TInternalState, TExternalState, TAction> gameLogic)
        {
            this.gameLogic = gameLogic;
            _state = new BehaviorSubject<(TInternalState state, TAction action)>((gameLogic.InitialState(), default(TAction)!));
            states = _state.Replay(1).RefCount();
            States = states.Select(tuple => (GetExternalState(tuple.state), tuple.action));

            states.Select(s => s.state).Select(s => Observable.FromAsync(() => CheckAutomaticActions(s))).Concat().Subscribe();
        }


        public JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings
        {
            Converters =
            {
                new StringEnumConverter(new CamelCaseNamingStrategy(), allowIntegerValues: false),
            },
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            },
        };
        public IObservable<(TExternalState state, TAction action)> States { get; }

        public TimeSpan SlidingExpiration => TimeSpan.FromHours(1);

        public async Task<bool> Do(TAction action)
        {
            var (next, valid) = await ApplyAction(_state.Value.state, action);
            if (valid)
            {
                _state.OnNext((next, action));
            }
            return valid;
        }


        private Task<(TInternalState newState, bool isValid)> ApplyAction(TInternalState state, TAction action)
        {
            return Task.FromResult(gameLogic.PerformAction(state, action, null));
        }

        private Task CheckAutomaticActions(TInternalState state)
        {
            var (action, hasAction) = gameLogic.GetRecommendedAction(state, null);
            return hasAction
                ? this.Do(action)
                : Task.CompletedTask;
        }

        private TExternalState GetExternalState(TInternalState state)
        {
            return gameLogic.ToPublicGameState(state, null);
        }
    }

    public static class InMemoryGame
    {
        public static InMemoryGame<TExternalState, TInternalState, TAction> CreateInMemoryGame<TExternalState, TInternalState, TAction>(this IGameLogic<TInternalState, TExternalState, TAction> gameLogic) =>
            new InMemoryGame<TExternalState, TInternalState, TAction>(gameLogic);
    }
}
