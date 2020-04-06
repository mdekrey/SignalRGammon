using SignalRGame.ClashOfClones.Rules;
using SignalRGammon.Clash;
using SignalRGammon.Clash.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SignalRGame.ClashOfClones
{
    public class DropLogicShould
    {
        [Fact]
        public void GenerateNewUnits()
        {
            var armyConfiguration = Defaults.DefaultArmyConfiguration;
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            var target = MakeTarkget(Enumerable.Repeat(0, armyConfiguration.MaxUnitCount).ToArray());

            var newArmyLayout = target.Recruit(blankArmyLayout, armyConfiguration);

            var counts = target.CurrentUnits(newArmyLayout).Sum(t => t.unitCount);
            Assert.Equal(armyConfiguration.MaxUnitCount, counts);
            Assert.Empty(ArmyLayoutMatcher.GetMatches(newArmyLayout));
        }

        [Fact]
        public void DropAUnitOnAnEmptyField()
        {
            var blankArmyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray());
            var target = MakeTarkget();

            var newArmyLayout = target.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));

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
            var target = MakeTarkget();

            var newArmyLayout = target.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = target.Drop(newArmyLayout, 2, new StandardUnit("foo2", 0, null));

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
            var target = MakeTarkget();

            var newArmyLayout = target.Drop(blankArmyLayout, 2, new StandardUnit("foo", 0, null));
            newArmyLayout = target.Drop(newArmyLayout, 2, new EliteUnit("foo2", 0, null, 0));

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

        private static Action<UnitPlaceholder>[] GetUnitAssertions(Action<(int column, int row), UnitPlaceholder> assertion)
        {
            return Enumerable.Range(0, ArmyLayout.TotalCount)
                             .Select(index => (Action<UnitPlaceholder>)(unit => assertion(ArmyLayout.GetPositionFor(index), unit)))
                             .ToArray();
        }

        private static DropLogic MakeTarkget(params int[] rolls)
        {
            var dieRoller = new FakeDieRoller(rolls);
            var target = new DropLogic(dieRoller);
            return target;
        }
    }
}
