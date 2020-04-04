using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SignalRGammon.Clash
{
    namespace StateComponents
    {
        [JsonConverter(typeof(JsonSubtypes), "type")]
        [JsonSubtypes.KnownSubType(typeof(EmptyPlaceholder), EmptyPlaceholder.TypeValue)]
        [JsonSubtypes.KnownSubType(typeof(StandardUnit), EmptyPlaceholder.TypeValue)]
        [JsonSubtypes.KnownSubType(typeof(EliteUnit), EmptyPlaceholder.TypeValue)]
        [JsonSubtypes.KnownSubType(typeof(ChampionUnit), EmptyPlaceholder.TypeValue)]
        [JsonSubtypes.KnownSubType(typeof(UnitPart), EmptyPlaceholder.TypeValue)]
        public abstract class UnitPlaceholder
        {
            /// <summary>
            /// Used by JsonConverter; this property is required for the type to be serialized.
            /// </summary>
            public abstract string Type { get; }
        }
        public class EmptyPlaceholder : UnitPlaceholder
        {
            public const string TypeValue = "empty";
            public override string Type => TypeValue;
        }
        public abstract class PlacedUnit : UnitPlaceholder
        {
            public string Id { get; }
            public PlacedUnit(string id)
            {
                this.Id = id;
            }
        }
        public class UnitPart : PlacedUnit
        {
            public const string TypeValue = "part";
            public override string Type => TypeValue;
            public UnitPart(string id) : base(id)
            {
            }
        }
        public class StandardUnit : PlacedUnit
        {
            public const string TypeValue = "standard";
            public override string Type => TypeValue;
            public StandardUnit(string id) : base(id)
            {
            }
            public int ColorId { get; set; }
        }
        public class EliteUnit : PlacedUnit
        {
            public const string TypeValue = "elite";
            public override string Type => TypeValue;
            public EliteUnit(string id) : base(id)
            {
            }
            public int ColorId { get; set; }
            public int ArmySpecialUnitIndex { get; set; }
        }
        public class ChampionUnit : PlacedUnit
        {
            public const string TypeValue = "champion";
            public override string Type => TypeValue;
            public ChampionUnit(string id) : base(id)
            {
            }
            public int ColorId { get; set; }
            public int ArmySpecialUnitIndex { get; set; }
        }




        public readonly struct ArmyLayout
        {
            public const int Rows = 6;
            public const int Columns = 7;
            public const int TotalCount = Rows * Columns;

            public IReadOnlyList<PlacedUnit> Units { get; }

            public ArmyLayout(IReadOnlyList<PlacedUnit> units)
            {
                if (units.Count != TotalCount)
                    throw new ArgumentException("Must have {TotalCount} unit slots", nameof(units));
                this.Units = units;
            }

            public PlacedUnit this[int column, int row] => Units[column + row * Columns];
        }
    }
}
