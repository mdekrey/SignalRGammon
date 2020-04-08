using System;
using System.Collections.Generic;
using System.Linq;
using SignalRGame.ClashOfClones.StateComponents;

namespace SignalRGame.ClashOfClones.Rules
{
    public class ArmyLayoutMatcher
    {
        public static IEnumerable<ArmyLayoutMatch> GetMatches(ArmyLayout armyLayout)
        {
            for (var column = 0; column < ArmyLayout.Columns; column++)
            {
                for (var row = 0; row < ArmyLayout.Rows; row++)
                {
                    switch (armyLayout[column, row])
                    {
                        case StandardUnit { ChargeState: null }:
                            if (IsLeftEndOfNewWall(armyLayout, column, row, out var wallMatch))
                                yield return wallMatch;
                            if (IsStartOfStandardFormation(armyLayout, column, row, out var formationMatch))
                                yield return formationMatch;
                            break;
                        case EliteUnit { ChargeState: null }:
                            if (IsStartOfEliteFormation(armyLayout, column, row, out var eliteMatch))
                                yield return eliteMatch;
                            break;
                        case ChampionUnit { ChargeState: null }:
                            if (IsStartOfChampionFormation(armyLayout, column, row, out var championMatch))
                                yield return championMatch;
                            break;
                        case WallUnit _: 
                            // TODO - combine walls
                        case EmptyPlaceholder _:
                        case UnitPart _:
                            continue;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        public static bool IsLeftEndOfNewWall(ArmyLayout armyLayout, int column, int row, out ArmyLayoutMatch match)
        {
            match = default;
            if (!(armyLayout[column, row] is StandardUnit { ColorId: var currentColor, ChargeState: null, Id: var startId }))
                return false; // was not an uncharged standard unit
            if (column > 0
                && armyLayout[column - 1, row] is StandardUnit { ChargeState: null, ColorId: var prevColor }
                && prevColor == currentColor)
                return false; // to the left was an uncharged unit of the same color; wall is further left

            var ids = Enumerable.Range(column, ArmyLayout.Columns)
                .TakeWhile(column => column < ArmyLayout.Columns)
                .Select(column => armyLayout[column, row])
                .TakeWhile(unit => unit is StandardUnit { ChargeState: null, ColorId: var nextColor } && nextColor == currentColor)
                .Select(unit => ((StandardUnit)unit).Id)
                .ToArray();
            if (ids.Length < 3)
                return false;
            match = new ArmyLayoutMatch(ids, true, column, row);
            return true;
        }

        public static bool IsStartOfStandardFormation(ArmyLayout armyLayout, int column, int row, out ArmyLayoutMatch match)
        {
            match = default;
            if (!(armyLayout[column, row] is StandardUnit { ColorId: var currentColor, ChargeState: null, Id: var startId }))
                return false; // was not an uncharged standard unit
            if (row > 0
                && armyLayout[column, row - 1] is StandardUnit { ChargeState: null, ColorId: var prevColor }
                && prevColor == currentColor)
                return false; // to the front was an uncharged unit of the same color; front of formation is further up

            var ids = GetStandardUnitIds(armyLayout, currentColor, column, row + 1);
            if (ids.Length < 2)
                return false;
            match = new ArmyLayoutMatch(new[] { startId }.Concat(ids).ToArray(), false, column, row);
            return true;
        }

        private static string[] GetStandardUnitIds(ArmyLayout armyLayout, int currentColor, int column, int row)
        {
            return Enumerable.Range(row, 2)
                .Where(row => row < ArmyLayout.Rows)
                .Select(row => armyLayout[column, row])
                .TakeWhile(unit => unit is StandardUnit { ChargeState: null, ColorId: var nextColor } && nextColor == currentColor)
                .Select(unit => ((StandardUnit)unit).Id)
                .ToArray();
        }

        public static bool IsStartOfEliteFormation(ArmyLayout armyLayout, int column, int row, out ArmyLayoutMatch match)
        {
            match = default;
            if (!(armyLayout[column, row] is EliteUnit { ColorId: var currentColor, ChargeState: null, Id: var startId }))
                return false; // was not an uncharged elite unit

            var ids = GetStandardUnitIds(armyLayout, currentColor, column, row + 2);
            if (ids.Length < 2)
                return false;
            match = new ArmyLayoutMatch(new[] { startId }.Concat(ids).ToArray(), false, column, row);
            return true;
        }

        public static bool IsStartOfChampionFormation(ArmyLayout armyLayout, int column, int row, out ArmyLayoutMatch match)
        {
            match = default;
            if (!(armyLayout[column, row] is ChampionUnit { ColorId: var currentColor, ChargeState: null, Id: var startId }))
                return false; // was not an uncharged champion unit

            var ids = GetStandardUnitIds(armyLayout, currentColor, column, row + 2);
            if (ids.Length < 2)
                return false;
            var col2Ids = GetStandardUnitIds(armyLayout, currentColor, column + 1, row + 2);
            if (col2Ids.Length < 2)
                return false;
            match = new ArmyLayoutMatch(new[] { startId }.Concat(ids).Concat(col2Ids).ToArray(), false, column, row);
            return true;
        }
    }

    public readonly struct ArmyLayoutMatch
    {
        public IReadOnlyList<string> UnitIds { get; }
        public bool IsWall { get; }
        public int Column { get; }
        public int Row { get; }

        public ArmyLayoutMatch(IReadOnlyList<string> ids, bool isWall, int column, int row)
        {
            this.UnitIds = ids;
            this.IsWall = isWall;
            this.Column = column;
            this.Row = row;
        }
    }
}