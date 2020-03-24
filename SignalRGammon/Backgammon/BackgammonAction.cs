using JsonSubTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGammon.Backgammon
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(BackgammonDiceRoll), BackgammonDiceRoll.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonMove), BackgammonMove.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonBearOff), BackgammonBearOff.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonUndo), BackgammonUndo.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonSetStartingPlayer), BackgammonSetStartingPlayer.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonCannotUseRoll), BackgammonCannotUseRoll.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonDeclareWinner), BackgammonDeclareWinner.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(BackgammonNewGame), BackgammonNewGame.TypeValue)]
    public abstract class BackgammonAction
    {
        /// <summary>
        /// Used by JsonConverter; this property is required for the type to be serialized.
        /// </summary>
        public abstract string Type { get; }
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

    public class BackgammonUndo : BackgammonAction
    {
        public const string TypeValue = "undo";
        public override string Type => TypeValue;
    }


    public class BackgammonSetStartingPlayer : BackgammonAction
    {
        public const string TypeValue = "start";
        public override string Type => TypeValue;
    }

    public class BackgammonCannotUseRoll : BackgammonAction
    {
        public const string TypeValue = "cannot-use-roll";
        public override string Type => TypeValue;
        public IEnumerable<int> DieValues { get; set; } = Enumerable.Empty<int>();
    }

    public class BackgammonDeclareWinner : BackgammonAction
    {
        public const string TypeValue = "declare-winner";
        public override string Type => TypeValue;
        public Player Player { get; set; }
    }

    public class BackgammonNewGame : BackgammonAction
    {
        public const string TypeValue = "new-game";
        public override string Type => TypeValue;
    }
}