using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGame.Checkers
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(CheckersReady), CheckersReady.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(CheckersCannotMove), CheckersCannotMove.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(CheckersMove), CheckersMove.TypeValue)]
    [JsonSubtypes.KnownSubType(typeof(CheckersNewGame), CheckersNewGame.TypeValue)]
    public abstract class CheckersAction
    {
        /// <summary>
        /// Used by JsonConverter; this property is required for the type to be serialized.
        /// </summary>
        public abstract string Type { get; }
    }

    public class CheckersReady : CheckersAction
    {
        public const string TypeValue = "ready";
        public override string Type => TypeValue;
        public Player Player { get; set; }
    }

    public class CheckersMove : CheckersAction
    {
        public const string TypeValue = "move";
        public override string Type => TypeValue;
        public Player Player { get; set; }
        public int PieceIndex { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }
    }

    public class CheckersCannotMove : CheckersAction
    {
        public const string TypeValue = "no-moves";
        public override string Type => TypeValue;
    }

    public class CheckersNewGame : CheckersAction
    {
        public const string TypeValue = "new-game";
        public override string Type => TypeValue;
    }
}
