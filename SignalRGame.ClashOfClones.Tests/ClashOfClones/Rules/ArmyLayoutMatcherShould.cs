using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SignalRGame.ClashOfClones.Rules
{
    public class ArmyLayoutMatcherShould
    {
        [Fact]
        public void DetectMultipleMatches()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(2, new StandardUnit(NewId(), 0, null))
                .Drop(3, new ChampionUnit(NewId(), 1, null, 1))
                .Drop(5, new EliteUnit(NewId(), 2, null, 1))
                .Drop(3, new StandardUnit(NewId(), 1, null))
                .Drop(3, new StandardUnit(NewId(), 1, null))
                .Drop(4, new StandardUnit(NewId(), 1, null))
                .Drop(4, new StandardUnit(NewId(), 1, null))
                .Drop(5, new StandardUnit(NewId(), 2, null))
                .Drop(5, new StandardUnit(NewId(), 2, null));
            var units = armyLayout.Units.ToList();

            var matches = ArmyLayoutMatcher.GetMatches(armyLayout).OrderBy(m =>
            {
                var pos = ArmyLayout.GetPositionFor(units.FindIndex(u => u is UnitInstance ui && ui.Id == m.UnitIds[0]));
                return (pos.column, pos.row, m.IsWall);
            }).ToArray();

            Assert.Collection(matches,
                m => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, m.UnitIds[0]),
                m => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, m.UnitIds[0]),
                m => Assert.Equal(((UnitInstance)armyLayout[3, 0]).Id, m.UnitIds[0]),
                m => Assert.Equal(((UnitInstance)armyLayout[5, 0]).Id, m.UnitIds[0])
            );
        }


        [Fact]
        public void DetectStandardMatches()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.True(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 0, out var match));
            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 1, out var _));
            Assert.False(match.IsWall);
            Assert.Collection(match.UnitIds,
                id => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 1]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 2]).Id, id)
            );
        }

        [Fact]
        public void RejectIncompleteFormations()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectFormationsOfAlternateColors()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 1, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectFormationsOfChargedLeads()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, new ChargeState()))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectStandardFormationsStartingIncorrectly()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 1, out var _));
        }

        [Fact]
        public void DetectWallMatches()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(2, new StandardUnit(NewId(), 0, null));

            Assert.True(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 0, 0, out var match));
            Assert.True(match.IsWall);
            Assert.Collection(match.UnitIds,
                id => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[1, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[2, 0]).Id, id)
            );
        }

        [Fact]
        public void DetectLongWallMatches()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(2, new StandardUnit(NewId(), 0, null))
                .Drop(3, new StandardUnit(NewId(), 0, null));

            Assert.True(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 0, 0, out var match));
            Assert.False(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 1, 0, out var _));
            Assert.True(match.IsWall);
            Assert.Collection(match.UnitIds,
                id => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[1, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[2, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[3, 0]).Id, id)
            );
        }

        [Fact]
        public void RejectShortWalls()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectWallsOfAlternateColors()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(2, new StandardUnit(NewId(), 1, null));

            Assert.False(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectWallsOfChargedLeads()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, new ChargeState()))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(2, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsLeftEndOfNewWall(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void DetectEliteFormations()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.True(ArmyLayoutMatcher.IsStartOfEliteFormation(armyLayout, 0, 0, out var match));
            Assert.False(match.IsWall);
            Assert.Collection(match.UnitIds,
                id => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 2]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 3]).Id, id)
            );
        }

        [Fact]
        public void RejectIncompleteEliteFormations()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfEliteFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectEliteFormationsOfAlternateColors()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 1, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfEliteFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectEliteFormationsOfChargedLeads()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, new ChargeState(), 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfEliteFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectEliteFormationsStartingIncorrectly()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 1, out var _));
        }

        [Fact]
        public void DetectChampionFormations()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.True(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var match));
            Assert.False(match.IsWall);
            Assert.Collection(match.UnitIds,
                id => Assert.Equal(((UnitInstance)armyLayout[0, 0]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 2]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[0, 3]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[1, 2]).Id, id),
                id => Assert.Equal(((UnitInstance)armyLayout[1, 3]).Id, id)
            );
        }

        [Fact]
        public void RejectIncompleteChampionFormationsRow()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectIncompleteChampionFormationsColumn()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectIncompleteChampionFormationsThreeOfFour()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectChampionFormationsOfAlternateColors()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 1, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectChampionFormationsOfChargedLeads()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new ChampionUnit(NewId(), 0, new ChargeState(), 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfChampionFormation(armyLayout, 0, 0, out var _));
        }

        [Fact]
        public void RejectChampionFormationsStartingIncorrectly()
        {
            var armyLayout = new ArmyLayout(Enumerable.Repeat<UnitPlaceholder>(new EmptyPlaceholder(), ArmyLayout.TotalCount).ToArray())
                .Drop(0, new EliteUnit(NewId(), 0, null, 0))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(0, new StandardUnit(NewId(), 0, null))
                .Drop(1, new EliteUnit(NewId(), 0, null, 0))
                .Drop(1, new StandardUnit(NewId(), 0, null))
                .Drop(1, new StandardUnit(NewId(), 0, null));

            Assert.False(ArmyLayoutMatcher.IsStartOfStandardFormation(armyLayout, 0, 1, out var _));
        }


        private string NewId() => Guid.NewGuid().ToString();
    }
}
