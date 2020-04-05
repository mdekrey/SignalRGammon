using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGammon.Clash
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(ClashReady), ClashReady.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(ClashMove), ClashMove.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(ClashNewGame), ClashNewGame.TypeValue)]
    public abstract class ClashAction
    {
        /// <summary>
        /// Used by JsonConverter; this property is required for the type to be serialized.
        /// </summary>
        public abstract string Type { get; }
    }

    public class ClashReady : ClashAction
    {
        public const string TypeValue = "ready";
        public override string Type => TypeValue;
        public Player Player { get; set; }
    }

    public class ClashMove : ClashAction
    {
        public const string TypeValue = "move";
        public override string Type => TypeValue;
        public Player Player { get; set; }
        public int PieceIndex { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }
    }

    public class ClashNewGame : ClashAction
    {
        public const string TypeValue = "new-game";
        public override string Type => TypeValue;
    }
}
