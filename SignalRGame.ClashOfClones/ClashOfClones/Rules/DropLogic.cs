using SignalRGame.GameUtilities;
using SignalRGame.ClashOfClones.StateComponents;
using SignalRGame.ClashOfClones;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SignalRGame.ClashOfClones.Rules
{
    public class DropLogic
    {
        private readonly IDieRoller dieRoller;

        public DropLogic(IDieRoller dieRoller)
        {
            this.dieRoller = dieRoller;
        }

        public IEnumerable<(PlacedUnit primary, int unitCount)> CurrentUnits(ArmyLayout armyLayout)
        {
            return (from unit in armyLayout.Units.OfType<PlacedUnit>()
                    group unit by unit.Id into unit
                    let primary = unit.Single(u => u is UnitInstance || u is WallUnit)
                    select (primary, unitCount: ShouldCountParts(primary) ? unit.Count() : 1)
                    ).ToArray();
        }

        private bool ShouldCountParts(PlacedUnit unit)
        {
            return unit switch
            {
                StandardUnit _ => true,
                EliteUnit _ => false,
                ChampionUnit _ => false,
                WallUnit _ => false,
                _ => throw new NotImplementedException("Could not handle unit type: " + unit.Type)
            };
        }

        public ArmyLayout Recruit(ArmyLayout armyLayout, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            var initialUnits = CurrentUnits(armyLayout);
            var newCount = armyConfiguration.MaxUnitCount - initialUnits.Sum(u => u.unitCount);
            var highRankCount = initialUnits.Where(u => u.primary is EliteUnit).Count()
                + 2 * initialUnits.Where(u => u.primary is ChampionUnit).Count();
            for (var i = newCount; i > 0; i--)
            {
                var columnOptions = GetColumnOptions(armyLayout).ToArray();
                var column = columnOptions[dieRoller.RollDie(columnOptions.Length)];
                var options = (from option in GetWeightedOptions(column, armyLayout, armyConfiguration, gameSettings, highRankCount)
                               from entry in Enumerable.Repeat(option.unitFactory, option.odds)
                               select entry
                              ).ToArray();
                var unitFactory = options[dieRoller.RollDie(options.Length)];
                var unit = unitFactory();

                if (unit is EliteUnit) highRankCount++;
                if (unit is ChampionUnit) highRankCount += 2;
                armyLayout = Drop(armyLayout, column, unit);
            }
            return armyLayout;
        }

        public static IEnumerable<int> GetColumnOptions(ArmyLayout armyLayout)
        {
            for (var column = 0; column < ArmyLayout.Columns; column++)
            {
                if (!armyLayout.IsEmpty(column, ArmyLayout.Rows - 1))
                    continue;
                yield return column;
            }
        }

        public static IEnumerable<(Func<UnitInstance> unitFactory, int odds)> GetWeightedOptions(int column, ArmyLayout armyLayout, ArmyConfiguration armyConfiguration, GameSettings gameSettings, int highRankCount)
        {
            for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
            {
                var color = c;
                if (ArmyLayoutMatcher.GetMatches(Drop(armyLayout, column, new StandardUnit("", color, null))).Any())
                {
                    continue;
                }
                yield return (() => new StandardUnit(MakeUnitId(), color, null), armyConfiguration.MaxUnitCount);
            }

            if (highRankCount < 4 && armyLayout.IsEmpty(column, ArmyLayout.Rows - 2))
            {
                for (var i = 0; i < armyConfiguration.SpecialUnits.Count; i++)
                {
                    var specialUnitIndex = i;
                    var specialUnit = armyConfiguration.SpecialUnits[specialUnitIndex];
                    if (gameSettings.Elites.ContainsKey(specialUnit.UnitId))
                    {
                        for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
                        {
                            var color = c;

                            yield return (() => new EliteUnit(MakeUnitId(), color, null, specialUnitIndex), specialUnit.Stock);
                        }
                    }
                }
            }

            if (column < ArmyLayout.Columns - 1)
            {
                if (highRankCount < 3 && armyLayout.IsEmpty(column, ArmyLayout.Rows - 2) && armyLayout.IsEmpty(column + 1, ArmyLayout.Rows - 2) && armyLayout.IsEmpty(column + 1, ArmyLayout.Rows - 1))
                {
                    for (var i = 0; i < armyConfiguration.SpecialUnits.Count; i++)
                    {
                        var specialUnitIndex = i;
                        var specialUnit = armyConfiguration.SpecialUnits[specialUnitIndex];
                        if (gameSettings.Champions.ContainsKey(specialUnit.UnitId))
                        {
                            for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
                            {
                                var color = c;

                                yield return (() => new ChampionUnit(MakeUnitId(), color, null, specialUnitIndex), specialUnit.Stock);
                            }
                        }
                    }
                }
            }
        }

        private static string MakeUnitId() => Guid.NewGuid().ToString();

        public static ArmyLayout Drop(ArmyLayout armyLayout, int column, UnitInstance unit)
        {
            var units = armyLayout.Units.ToImmutableList().ToBuilder();
            switch (unit)
            {
                case StandardUnit _:
                    {
                        var row = Enumerable.Range(0, ArmyLayout.Rows - 1)
                            .Where(row => !armyLayout.IsEmpty(column, row))
                            .DefaultIfEmpty(-1)
                            .Select(row => row + 1)
                            .Max();
                        units[ArmyLayout.GetIndexFor(column, row)] = unit;
                    }
                    break;
                case EliteUnit _:
                    {
                        var row = Enumerable.Range(0, ArmyLayout.Rows - 1)
                            .Where(row => !armyLayout.IsEmpty(column, row))
                            .DefaultIfEmpty(-1)
                            .Select(row => row + 1)
                            .Max();
                        units[ArmyLayout.GetIndexFor(column, row)] = unit;
                        units[ArmyLayout.GetIndexFor(column, row + 1)] = new UnitPart(unit.Id);
                    }
                    break;
                case ChampionUnit _:
                    {
                        var row = Enumerable.Range(0, ArmyLayout.Rows - 1)
                            .Where(row => !armyLayout.IsEmpty(column, row) || !armyLayout.IsEmpty(column + 1, row))
                            .DefaultIfEmpty(-1)
                            .Select(row => row + 1)
                            .Max();
                        units[ArmyLayout.GetIndexFor(column, row)] = unit;
                        units[ArmyLayout.GetIndexFor(column, row + 1)] = new UnitPart(unit.Id);
                        units[ArmyLayout.GetIndexFor(column + 1, row)] = new UnitPart(unit.Id);
                        units[ArmyLayout.GetIndexFor(column + 1, row + 1)] = new UnitPart(unit.Id);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return new ArmyLayout(units.ToImmutable());
        }
    }
}
