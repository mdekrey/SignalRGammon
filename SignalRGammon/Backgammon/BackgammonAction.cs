using JsonSubTypes;
using Newtonsoft.Json;

namespace SignalRGammon.Backgammon
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(BackgammonDiceRoll), "roll")]
    [JsonSubtypes.KnownSubType(typeof(BackgammonMove), "move")]
    public class BackgammonAction
    {
        public Player Player { get; set; }
    }

    public class BackgammonDiceRoll : BackgammonAction
    {
    }

    public class BackgammonMove : BackgammonAction
    {
        public int DieValue { get; set; }
        public int StartingPointNumber { get; set; }
    }
}