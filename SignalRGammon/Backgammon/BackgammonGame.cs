using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SignalRGammon.Backgammon
{
    public class BackgammonGame : IGame<BackgammonState, BackgammonAction>
    {
        private static readonly JsonSerializerSettings BackgammonJsonSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new StringEnumConverter(),
            },
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            },
        };

        private readonly BehaviorSubject<BackgammonState> state = new BehaviorSubject<BackgammonState>(BackgammonState.DefaultState(new DieRoller()));

        public JsonSerializerSettings JsonSettings => BackgammonJsonSettings;

        public IObservable<BackgammonState> States => state.AsObservable();

        public TimeSpan SlidingExpiration => TimeSpan.FromHours(1);

        public async Task<bool> Do(BackgammonAction action)
        {
            var (next, valid) = await state.Value.ApplyAction(action);
            if (valid)
            {
                state.OnNext(next);
            }
            return valid;
        }
    }
}
