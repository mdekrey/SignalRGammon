using SignalRGame.GameUtilities;
using SignalRGammon.Clash.StateComponents;
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

        public ArmyLayout Recruit(ArmyLayout armyLayout, ArmyConfiguration armyConfiguration)
        {
            var initialUnits = CurrentUnits(armyLayout);
            var newCount = armyConfiguration.MaxUnitCount - initialUnits.Sum(u => u.unitCount);
            var eliteCount = initialUnits.Where(u => u.primary is EliteUnit).Count();
            var championCount = initialUnits.Where(u => u.primary is ChampionUnit).Count();
            for (var i = newCount; i > 0; i--)
            {
                var options = (from option in GetWeightedOptions(armyLayout, armyConfiguration)
                               from entry in Enumerable.Repeat((option.column, option.unitFactory), option.odds)
                               select entry
                              ).ToArray();
                var (column, unitFactory) = options[dieRoller.RollDie(options.Length)];
                var unit = unitFactory();

                if (unit is EliteUnit) eliteCount++;
                if (unit is ChampionUnit) eliteCount++;
                armyLayout = Drop(armyLayout, column, unit);
            }
            return armyLayout;

            IEnumerable<(int column, Func<UnitInstance> unitFactory, int odds)> GetWeightedOptions(ArmyLayout armyLayout, ArmyConfiguration armyConfiguration)
            {
                for (var column = 0; column < ArmyLayout.Columns; column++)
                {
                    if (!(armyLayout[column, ArmyLayout.Rows - 1] is EmptyPlaceholder))
                        continue;

                    for (var color = 0; color < ArmyConfiguration.ColorCount; color++)
                    {
                        var c = color;
                        // TODO - check to make sure this won't complete anything
                        if (ArmyLayoutMatcher.GetMatches(Drop(armyLayout, column, new StandardUnit("", c, null))).Any())
                        {
                            continue;
                        }
                        yield return (column, () => new StandardUnit(MakeUnitId(), c, null), armyConfiguration.MaxUnitCount);
                    }

                    // TODO - elites

                    if (column < ArmyLayout.Columns - 1)
                    {
                        // TODO - champions
                    }
                }
            }
        }

        private string MakeUnitId() => Guid.NewGuid().ToString();

        public ArmyLayout Drop(ArmyLayout armyLayout, int column, UnitInstance unit)
        {
            var units = armyLayout.Units.ToImmutableList().ToBuilder();
            switch (unit)
            {
                case StandardUnit _:
                    {
                        var row = Enumerable.Range(0, ArmyLayout.Rows).Where(row => armyLayout[column, row] is EmptyPlaceholder).Min();
                        units[ArmyLayout.GetIndexFor(column, row)] = unit;
                    }
                    break;
                case EliteUnit _:
                    {
                        var row = Enumerable.Range(0, ArmyLayout.Rows - 1).Where(row => armyLayout[column, row] is EmptyPlaceholder).Min();
                        units[ArmyLayout.GetIndexFor(column, row)] = unit;
                        units[ArmyLayout.GetIndexFor(column, row + 1)] = new UnitPart(unit.Id);
                    }
                    break;
                case ChampionUnit _:
                default:
                    throw new NotImplementedException();
            }
            return new ArmyLayout(units.ToImmutable());
        }
    }
}
