using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static SignalRGame.ClashOfClones.Rules.ArmyLayoutAssertionUtilities;

namespace SignalRGame.ClashOfClones.Rules
{
    public class ApplyMatchLogicShould
    {
        [Fact]
        public void ProduceStandardFormations()
        {
            string id1, id2, id3;
            var color = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(id1 = NewId(), color, null))
                .Drop(0, new StandardUnit(id2 = NewId(), color, null))
                .Drop(0, new StandardUnit(id3 = NewId(), color, null))
                .Drop(0, new StandardUnit(NewId(), color, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id1, id2, id3 }, false, 0, 0)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (0, 1):
                            case (0, 2):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (0, 3):
                                {
                                    Assert.Equal(armyLayout[0, 3], unit);
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
        public void ProduceWalls()
        {
            string id1, id2, id3, id4;
            var color = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(id1 = NewId(), color, null))
                .Drop(1, new StandardUnit(id2 = NewId(), color, null))
                .Drop(2, new StandardUnit(id3 = NewId(), color, null))
                .Drop(3, new StandardUnit(id4 = NewId(), color, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id1, id2, id3, id4 }, true, 0, 0)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (1, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id3, actual.Id);
                                    break;
                                }
                            case (3, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id4, actual.Id);
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
        public void ProduceChargedElites()
        {
            string id1, id2, id3;
            var color = 0;
            var armySpecialUnitIndex = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(id1 = NewId(), color, null, armySpecialUnitIndex))
                .Drop(0, new StandardUnit(id2 = NewId(), color, null))
                .Drop(0, new StandardUnit(id3 = NewId(), color, null))
                .Drop(0, new StandardUnit(NewId(), color, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id1, id2, id3 }, false, 0, 0)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 0):
                                {
                                    var actual = Assert.IsType<EliteUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id1, actual.Id);
                                    Assert.Equal(armySpecialUnitIndex, actual.ArmySpecialUnitIndex);
                                    break;
                                }
                            case (0, 1):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (0, 4):
                                {
                                    // doesn't automatically fall/advance here...
                                    Assert.Equal(armyLayout[0, 4], unit);
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
        public void ProduceChargedChampions()
        {
            string id1, id2, id3, id4, id5;
            var color = 0;
            var armySpecialUnitIndex = 1;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(id1 = NewId(), color, null, armySpecialUnitIndex))
                .Drop(0, new StandardUnit(id2 = NewId(), color, null))
                .Drop(0, new StandardUnit(id3 = NewId(), color, null))
                .Drop(1, new StandardUnit(id4 = NewId(), color, null))
                .Drop(1, new StandardUnit(id5 = NewId(), color, null))
                .Drop(0, new StandardUnit(NewId(), color, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id1, id2, id3, id4, id5 }, false, 0, 0)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 0):
                                {
                                    var actual = Assert.IsType<ChampionUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id1, actual.Id);
                                    Assert.Equal(armySpecialUnitIndex, actual.ArmySpecialUnitIndex);
                                    break;
                                }
                            case (0, 1):
                            case (1, 0):
                            case (1, 1):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (0, 4):
                                {
                                    // doesn't automatically fall/advance here...
                                    Assert.Equal(armyLayout[0, 4], unit);
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
        public void ProducesAWallStandardCombo()
        {
            string id1, id2, id3, id4, id5;
            var color = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(id1 = NewId(), color, null))
                .Drop(1, new StandardUnit(id2 = NewId(), color, null))
                .Drop(1, new StandardUnit(id3 = NewId(), color, null))
                .Drop(1, new StandardUnit(id4 = NewId(), color, null))
                .Drop(2, new StandardUnit(id5 = NewId(), color, null))
                .Drop(1, new StandardUnit(NewId(), color + 1, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id2, id3, id4 }, false, 1, 0),
                new ArmyLayoutMatch(new[] { id1, id2, id5 }, true, 0, 0)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (1, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    break;
                                }
                            case (2, 0):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id5, actual.Id);
                                    break;
                                }
                            case (1, 1):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (1, 2):
                            case (1, 3):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (1, 4):
                                {
                                    Assert.Equal(armyLayout[1, 3], unit);
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
        public void ProducesASecondRowWallStandardCombo()
        {
            string id1, id2, id3, id4, id5;
            var color = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), color + 1, null))
                .Drop(2, new StandardUnit(NewId(), color + 1, null))
                .Drop(0, new StandardUnit(id1 = NewId(), color, null))
                .Drop(1, new StandardUnit(id2 = NewId(), color, null))
                .Drop(1, new StandardUnit(id3 = NewId(), color, null))
                .Drop(1, new StandardUnit(id4 = NewId(), color, null))
                .Drop(2, new StandardUnit(id5 = NewId(), color, null))
                .Drop(1, new StandardUnit(NewId(), color + 1, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id2, id3, id4 }, false, 1, 0),
                new ArmyLayoutMatch(new[] { id1, id2, id5 }, true, 0, 1)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 1):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (1, 1):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    break;
                                }
                            case (2, 1):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id5, actual.Id);
                                    break;
                                }
                            case (1, 0):
                                {
                                    var actual = Assert.IsType<StandardUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (1, 2):
                            case (1, 3):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (0, 0):
                            case (2, 0):
                                {
                                    Assert.Equal(armyLayout[position.column, position.row], unit);
                                    break;
                                }
                            case (1, 4):
                                {
                                    Assert.Equal(armyLayout[1, 3], unit);
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
        public void ProducesWallComboBehindElite()
        {
            string id1, id2, id3, id4, id5;
            var color = 0;
            var gameSettings = new GameSettings();
            var armyConfiguration = Defaults.StaffedArmyConfiguration;
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), color + 1, null))
                .Drop(2, new StandardUnit(NewId(), color + 1, null))
                .Drop(0, new StandardUnit(NewId(), color + 1, null))
                .Drop(2, new StandardUnit(NewId(), color + 1, null))
                .Drop(0, new StandardUnit(id1 = NewId(), color, null))
                .Drop(1, new EliteUnit(id2 = NewId(), color, null, 0))
                .Drop(1, new StandardUnit(id3 = NewId(), color, null))
                .Drop(1, new StandardUnit(id4 = NewId(), color, null))
                .Drop(2, new StandardUnit(id5 = NewId(), color, null))
                .Drop(1, new StandardUnit(NewId(), color + 1, null));

            var resultArmyLayout = ApplyMatchLogic.ApplyMatches(armyLayout, new[]
            {
                new ArmyLayoutMatch(new[] { id2, id3, id4 }, false, 1, 0),
                new ArmyLayoutMatch(new[] { id1, id2, id5 }, true, 0, 2)
            }, armyConfiguration, gameSettings);

            Assert.Collection(resultArmyLayout.Units,
                GetUnitAssertions(
                    (position, unit) =>
                    {
                        switch (position)
                        {
                            case (0, 2):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id1, actual.Id);
                                    break;
                                }
                            case (1, 2):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    break;
                                }
                            case (2, 2):
                                {
                                    var actual = Assert.IsType<WallUnit>(unit);
                                    Assert.Equal(id5, actual.Id);
                                    break;
                                }
                            case (1, 0):
                                {
                                    var actual = Assert.IsType<EliteUnit>(unit);
                                    Assert.NotNull(actual.ChargeState);
                                    Assert.Equal(color, actual.ColorId);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (1, 1):
                                {
                                    var actual = Assert.IsType<UnitPart>(unit);
                                    Assert.Equal(id2, actual.Id);
                                    break;
                                }
                            case (0, 0):
                            case (2, 0):
                            case (0, 1):
                            case (2, 1):
                            case (1, 4):
                                {
                                    Assert.Equal(armyLayout[position.column, position.row], unit);
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

    }
}
