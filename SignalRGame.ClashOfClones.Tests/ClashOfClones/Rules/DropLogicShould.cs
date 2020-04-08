using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static SignalRGame.ClashOfClones.Rules.ArmyLayoutAssertionUtilities;

namespace SignalRGame.ClashOfClones.Rules
{
    public class DropLogicShould
    {
        [Fact]
        public void GenerateNewUnits()
        {
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            var target = MakeTarget(Enumerable.Repeat<Func<int, int>>(_ => 0, armyConfiguration.MaxUnitCount * 2).ToArray());

            var newArmyLayout = target.Recruit(blankArmyLayout, armyConfiguration, gameSettings);

            var counts = target.CurrentUnits(newArmyLayout).Sum(t => t.unitCount);
            Assert.Equal(armyConfiguration.MaxUnitCount, counts);
            Assert.Empty(ArmyLayoutMatcher.GetMatches(newArmyLayout));
        }

        [Fact]
        public void GenerateNewUnitsMax()
        {
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            var target = MakeTarget(Enumerable.Range(0, armyConfiguration.MaxUnitCount * 2).Select(index => (Func<int, int>)(sides => index % 2 == 0 ? 0 : sides - 1)).ToArray());

            var newArmyLayout = target.Recruit(blankArmyLayout, armyConfiguration, gameSettings);

            var counts = target.CurrentUnits(newArmyLayout).Sum(t => t.unitCount);
            Assert.Equal(armyConfiguration.MaxUnitCount, counts);
            Assert.Empty(ArmyLayoutMatcher.GetMatches(newArmyLayout));
        }

        [Fact]
        public void GenerateNewUnitsWithHighRanks()
        {
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit("foo", 0, null, 0))
                .Drop(2, new EliteUnit("foo2", 0, null, 0));
            var target = MakeTarget(Enumerable.Range(0, armyConfiguration.MaxUnitCount * 2).Select(index => (Func<int, int>)(sides => index % 2 == 0 ? 0 : sides - 1)).ToArray());

            var newArmyLayout = target.Recruit(armyLayout, armyConfiguration, gameSettings);

            var counts = target.CurrentUnits(newArmyLayout).Sum(t => t.unitCount);
            Assert.Equal(armyConfiguration.MaxUnitCount, counts);
            Assert.Empty(ArmyLayoutMatcher.GetMatches(newArmyLayout));
        }

        [Fact]
        public void GetDropOptionsForBlankBattlefield()
        {
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var options = DropLogic.GetWeightedOptions(0, blankArmyLayout, armyConfiguration, gameSettings, 0).ToArray();

            for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
            {
                Assert.Contains(options, p => p.unitFactory() is StandardUnit { ColorId: var color, ChargeState: null } && c == color);
            }
            for (var i = 0; i < armyConfiguration.SpecialUnits.Count; i++)
            {
                for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
                {
                    if (gameSettings.Champions.ContainsKey(armyConfiguration.SpecialUnits[i].UnitId))
                        Assert.Contains(options, p => p.unitFactory() is ChampionUnit { ArmySpecialUnitIndex: var index, ColorId: var color, ChargeState: null } && i == index && c == color);
                    else 
                        Assert.Contains(options, p => p.unitFactory() is EliteUnit { ArmySpecialUnitIndex: var index, ColorId: var color, ChargeState: null } && i == index && c == color);
                }
            }
        }

        [Fact]
        public void GetDropOptionsForCrowdedBattlefield()
        {
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            for (var col = 0; col < ArmyLayout.Columns; col += 2)
            {
                for (var i = 0; i < ArmyLayout.Rows; i++)
                {
                    armyLayout = DropLogic.Drop(armyLayout, col, new StandardUnit(Guid.NewGuid().ToString(), i % ArmyConfiguration.ColorCount, null));
                }
            }

            var options = DropLogic.GetWeightedOptions(1, armyLayout, armyConfiguration, gameSettings, 0).ToArray();

            Assert.DoesNotContain(options, p => p.unitFactory() is StandardUnit { ColorId: 0 });
            Assert.DoesNotContain(options, p => p.unitFactory() is ChampionUnit);

            // row 0 will have the 0th color as above
            for (var c = 1; c < ArmyConfiguration.ColorCount; c++)
            {
                Assert.Contains(options, p => p.unitFactory() is StandardUnit { ColorId: var color, ChargeState: null } && c == color);
            }
            for (var i = 0; i < armyConfiguration.SpecialUnits.Count; i++)
            {
                for (var c = 0; c < ArmyConfiguration.ColorCount; c++)
                {
                    if (gameSettings.Elites.ContainsKey(armyConfiguration.SpecialUnits[i].UnitId))
                        Assert.Contains(options, p => p.unitFactory() is EliteUnit { ArmySpecialUnitIndex: var index, ColorId: var color, ChargeState: null } && i == index && c == color);
                }
            }
        }

        [Fact]
        public void DropAUnitOnAnEmptyField()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        [Fact]
        public void DropStackedUnits()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 2, new StandardUnit("foo2", 0, null));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            case (2, 1):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        [Fact]
        public void DropStackedElite()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 2, new EliteUnit("foo2", 0, null, 0));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            case (2, 1):
                                {
                                    var actual = Assert.IsType<EliteUnit>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            case (2, 2):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        [Fact]
        public void DropStackedChampion()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 2, new ChampionUnit("foo2", 0, null, 0));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            case (2, 1):
                                {
                                    var actual = Assert.IsType<ChampionUnit>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            case (2, 2):
                            case (3, 1):
                            case (3, 2):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        [Fact]
        public void DropStackedChampionOtherColumn()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());

            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 1, new ChampionUnit("foo2", 0, null, 0));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            case (1, 1):
                                {
                                    var actual = Assert.IsType<ChampionUnit>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            case (1, 2):
                            case (2, 1):
                            case (2, 2):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        [Fact]
        public void DropUnitOnStackedChampion()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            
            var newArmyLayout = DropLogic.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 2, new ChampionUnit("foo2", 0, null, 0));
            newArmyLayout = DropLogic.Drop(newArmyLayout, 3, new StandardUnit("foo3", 0, null));

            Assert.Collection(newArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo", actual.Id);
                                    break;
                                }
                            case (2, 1):
                                {
                                    var actual = Assert.IsType<ChampionUnit>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            case (2, 2):
                            case (3, 1):
                            case (3, 2):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal("foo2", actual.Id);
                                    break;
                                }
                            case (3, 3):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.Equal("foo3", actual.Id);
                                    break;
                                }
                            default:
                                Assert.IsType<EmptyPlaceholder>(unit);
                                break;
                        }
                    }
                )
            );
        }

        private static DropLogic MakeTarget(params Func<int, int>[] rolls)
        {
            var dieRoller = new FakeDieRoller(rolls);
            var target = new DropLogic(dieRoller);
            return target;
        }
    }
}
