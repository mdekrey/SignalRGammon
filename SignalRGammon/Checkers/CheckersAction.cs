using JsonSubTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SignalRGammon.Checkers
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    public abstract class CheckersAction
    {
        /// <summary>
        /// Used by JsonConverter; this property is required for the type to be serialized.
        /// </summary>
        public abstract string Type { get; }
    }
}
