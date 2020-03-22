using JsonSubTypes;
using Newtonsoft.Json;

namespace SignalRGammon.Backgammon
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(BackgammonDiceRoll), BackgammonDiceRoll.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonMove), BackgammonMove.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonBearOff), BackgammonBearOff.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonSetStartingPlayer), BackgammonSetStartingPlayer.TypeValue)]
    public abstract class BackgammonAction
    {
#nullable disable
        /// <summary>
        /// Used by JsonConverter; this property is required for the type to be serialized.
        /// </summary>
        public abstract string Type { get; }
#nullable restore
    }

    public class BackgammonDiceRoll : BackgammonAction
    {
        public const string TypeValue = "roll";
        public override string Type => TypeValue;
        public Player Player { get; set; }
    }

    public class BackgammonMove : BackgammonAction
    {
        public const string TypeValue = "move";
        public override string Type => TypeValue;
        public Player Player { get; set; }
        public int DieValue { get; set; }
        public int StartingPointNumber { get; set; }
    }

    public class BackgammonBearOff : BackgammonAction
    {
        public const string TypeValue = "bear-off";
        public override string Type => TypeValue;
        public Player Player { get; set; }
        public int DieValue { get; set; }
        public int StartingPointNumber { get; set; }
    }

    public class BackgammonSetStartingPlayer : BackgammonAction
    {
        public const string TypeValue = "start";
        public override string Type => TypeValue;
    }
}