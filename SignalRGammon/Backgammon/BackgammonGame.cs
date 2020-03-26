using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SignalRGammon.GameUtilities;

namespace SignalRGammon.Backgammon
{
    public class BackgammonGame : IGame<BackgammonState, BackgammonAction?>
    {
        private static readonly JsonSerializerSettings BackgammonJsonSettings = new JsonSerializerSettings
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
        private readonly Rules rules;
        private readonly BehaviorSubject<(BackgammonState state, BackgammonAction? action)> state;

        public BackgammonGame(IDieRoller dieRoller)
        {
            rules = new Rules(dieRoller);
            state = new BehaviorSubject<(BackgammonState state, BackgammonAction? action)>((BackgammonState.DefaultState(), null));
            States = state.Replay(1).RefCount();

            States.Select(s => s.state).Select(s => Observable.FromAsync(() => rules.CheckAutomaticActions(s, Do))).Concat().Subscribe();
        }

        public JsonSerializerSettings JsonSettings => BackgammonJsonSettings;

        public IObservable<(BackgammonState state, BackgammonAction? action)> States { get; }

        public TimeSpan SlidingExpiration => TimeSpan.FromHours(1);

        public async Task<bool> Do(BackgammonAction? action)
        {
            if (action == null)
                return false;
            await Task.Yield();
            var (next, valid) = rules.ApplyAction(state.Value.state, action);
            if (valid)
            {
                state.OnNext((next, action));
            }
            return valid;
        }
    }
}
