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
        [JsonSubtypes.KnownSubType(typeof(WallUnit), EmptyPlaceholder.TypeValue)]
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
        public abstract class UnitInstance : PlacedUnit
        {
            public int ColorId { get; }
            public ChargeState? ChargeState { get; }
            public UnitInstance(string id, int colorId, ChargeState? chargeState) : base(id)
            {
                ColorId = colorId;
                this.ChargeState = chargeState;
            }
        }
        public class StandardUnit : UnitInstance
        {
            public const string TypeValue = "standard";
            public override string Type => TypeValue;
            public StandardUnit(string id, int colorId, ChargeState? chargeState) : base(id, colorId, chargeState) { }
        }
        public class EliteUnit : UnitInstance
        {
            public const string TypeValue = "elite";
            public override string Type => TypeValue;
            public int ArmySpecialUnitIndex { get; }
            public EliteUnit(string id, int colorId, ChargeState? chargeState, int armySpecialUnitIndex) : base(id, colorId, chargeState)
            {
                ArmySpecialUnitIndex = armySpecialUnitIndex;
            }
        }
        public class ChampionUnit : UnitInstance
        {
            public const string TypeValue = "champion";
            public override string Type => TypeValue;
            public int ArmySpecialUnitIndex { get; }
            public ChampionUnit(string id, int colorId, ChargeState? chargeState, int armySpecialUnitIndex) : base(id, colorId, chargeState)
            {
                ArmySpecialUnitIndex = armySpecialUnitIndex;
            }
        }
        public class WallUnit : PlacedUnit
        {
            public const string TypeValue = "champion";
            public override string Type => TypeValue;
            public int Strength { get; }
            public WallUnit(string id, int strength) : base(id)
            {
                Strength = strength;
            }
        }
        public readonly struct ChargeState
        {
            public int FuseCount { get; }
            public int CurrentAttack { get; }
            public int TurnsRemaining { get; }
            public ChargeState(int fuseCount, int currentAttack, int turnsRemaining)
            {
                FuseCount = fuseCount;
                CurrentAttack = currentAttack;
                TurnsRemaining = turnsRemaining;
            }
        }




        public readonly struct ArmyLayout
        {
            public const int Rows = 6;
            public const int Columns = 7;
            public const int TotalCount = Rows * Columns;

            public IReadOnlyList<UnitPlaceholder> Units { get; }

            public ArmyLayout(IReadOnlyList<UnitPlaceholder> units)
            {
                if (units.Count != TotalCount)
                    throw new ArgumentException("Must have {TotalCount} unit slots", nameof(units));
                this.Units = units;
            }

            public UnitPlaceholder this[int column, int row] => Units[GetIndexFor(column, row)];

            public static int GetIndexFor(int column, int row)
            {
                return column + row * Columns;
            }

            public static (int column, int row) GetPositionFor(int index)
            {
                return (column: index % Columns, row: index / Columns);
            }
        }
    }
}
