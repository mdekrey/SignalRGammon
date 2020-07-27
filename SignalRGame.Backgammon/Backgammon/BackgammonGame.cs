using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using SignalRGame.GameUtilities;

namespace SignalRGame.Backgammon
{
    public record BackgammonPublicState
    {
        public BackgammonState State { get; init; }
        public BackgammonAction? Action { get; init; }
    }

    public class BackgammonGame : IGameLogic<BackgammonState, BackgammonPublicState, BackgammonAction?>
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = {
                new System.Text.Json.Serialization.JsonStringEnumConverter()
            }
        };
        private static readonly Newtonsoft.Json.JsonSerializerSettings newtonsoftSettings = new Newtonsoft.Json.JsonSerializerSettings
        {
            Converters =
            {
                new Newtonsoft.Json.Converters.StringEnumConverter(new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(), allowIntegerValues: false),
            },
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
            {
                NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(),
            },
        };
        private readonly Rules rules;

        public BackgammonGame(IDieRoller dieRoller)
        {
            rules = new Rules(dieRoller);
        }

        public BackgammonState InitialState() => new BackgammonState();

        public (BackgammonState newState, bool isValid) PerformAction(BackgammonState state, BackgammonAction? action, ClaimsPrincipal? user) => rules.ApplyAction(state, action);

        public (BackgammonAction? action, bool hasAction) GetRecommendedAction(BackgammonState state, ClaimsPrincipal? user) => rules.GetAutomaticActions(state);

        public BackgammonPublicState ToPublicGameState(BackgammonState state, BackgammonAction? action, ClaimsPrincipal? user) => new BackgammonPublicState { State = state, Action = action };

        public string FromState(BackgammonState state) => JsonSerializer.Serialize(state, options);
        public BackgammonState ToState(string state) => JsonSerializer.Deserialize<BackgammonState>(state, options)!;
        public BackgammonAction? ToAction(string action) => Newtonsoft.Json.JsonConvert.DeserializeObject<BackgammonAction?>(action, newtonsoftSettings);
        public string FromPublicState(BackgammonPublicState state) => Newtonsoft.Json.JsonConvert.SerializeObject(state, newtonsoftSettings);
    }
}
