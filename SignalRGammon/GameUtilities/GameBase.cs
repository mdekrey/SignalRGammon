using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SignalRGammon.GameUtilities
{
    public abstract class GameBase<TExternalState, TInternalState, TAction> : IGame<TExternalState, TAction>
    {
        protected readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
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
        private readonly BehaviorSubject<(TInternalState state, TAction action)> _state;
        protected readonly IObservable<(TInternalState state, TAction action)> states;

        public GameBase(TInternalState defaultState)
        {
            _state = new BehaviorSubject<(TInternalState state, TAction action)>((defaultState, default(TAction)!));
            states = _state.Replay(1).RefCount();
            States = states.Select(tuple => (GetExternalState(tuple.state), tuple.action));

            states.Select(s => s.state).Select(s => Observable.FromAsync(() => CheckAutomaticActions(s))).Concat().Subscribe();
        }


        public JsonSerializerSettings JsonSettings => jsonSerializerSettings;
        public IObservable<(TExternalState state, TAction action)> States { get; }

        public virtual TimeSpan SlidingExpiration => TimeSpan.FromHours(1);

        public async Task<bool> Do(TAction action)
        {
            var (next, valid) = await ApplyAction(_state.Value.state, action);
            if (valid)
            {
                _state.OnNext((next, action));
            }
            return valid;
        }

        protected abstract TExternalState GetExternalState(TInternalState state);
        protected abstract Task<(TInternalState newState, bool isValid)> ApplyAction(TInternalState state, TAction action);
        protected abstract Task CheckAutomaticActions(TInternalState state);
    }
}
