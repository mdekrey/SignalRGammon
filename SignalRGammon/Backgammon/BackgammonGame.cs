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
    public class BackgammonGame : IGame<BackgammonState, BackgammonAction?>
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

        private readonly BehaviorSubject<(BackgammonState state, BackgammonAction? action)> state;

        public BackgammonGame(IDieRoller dieRoller)
        {
            state = new BehaviorSubject<(BackgammonState state, BackgammonAction? action)>((BackgammonState.DefaultState(dieRoller), null));

            state.Select(s => s.state).Select(s => Observable.FromAsync(() => CheckAutomaticActions(s))).Concat().Subscribe();
        }

        private async Task CheckAutomaticActions(BackgammonState obj)
        {
            switch (obj)
            {
                case { CurrentPlayer: null, DiceRolls: { White: var white, Black: var black } } when white.Count == 1 && black.Count == 1:
                    await Do(new BackgammonSetStartingPlayer 
                    { 
                        Player = white[0] > black[0] ? Player.White
                                : white[0] < black[0] ? Player.Black
                                : (Player?)null
                    });
                    break;
            }
        }

        public JsonSerializerSettings JsonSettings => BackgammonJsonSettings;

        public IObservable<(BackgammonState state, BackgammonAction? action)> States => state.AsObservable();

        public TimeSpan SlidingExpiration => TimeSpan.FromHours(1);

        public async Task<bool> Do(BackgammonAction? action)
        {
            if (action == null)
                return false;
            var (next, valid) = await state.Value.state.ApplyAction(action);
            if (valid)
            {
                state.OnNext((next, action));
            }
            return valid;
        }
    }
}
