using SignalRGame.GameUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRGame.Checkers
{
    public class CheckersGame : IGameLogic<CheckersState, CheckersExternalState, CheckersAction?>
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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

        public (CheckersAction? action, bool hasAction) GetRecommendedAction(CheckersState state, ClaimsPrincipal? user) => Rules.GetAutomaticAction(state);

        public CheckersState InitialState() => new CheckersState();

        public (CheckersState newState, bool isValid) PerformAction(CheckersState state, CheckersAction? action, ClaimsPrincipal? user) => Rules.ApplyAction(state, action);

        public CheckersExternalState ToPublicGameState(CheckersState state, CheckersAction? action, ClaimsPrincipal? user) => new CheckersExternalState(state, action);


        public string FromState(CheckersState state) => JsonSerializer.Serialize(state, options);
        public CheckersState ToState(string state) => JsonSerializer.Deserialize<CheckersState>(state, options);
        public CheckersAction? ToAction(string action) => Newtonsoft.Json.JsonConvert.DeserializeObject<CheckersAction?>(action, newtonsoftSettings);
        public string FromPublicState(CheckersExternalState state) => Newtonsoft.Json.JsonConvert.SerializeObject(state, newtonsoftSettings);
    }
}
