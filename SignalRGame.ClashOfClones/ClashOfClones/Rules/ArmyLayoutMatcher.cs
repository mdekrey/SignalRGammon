using System;
using System.Collections.Generic;
using System.Linq;
using SignalRGammon.Clash.StateComponents;

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
                        // TODO - elites
                        // TODO - champions
                        default: continue;
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
            if (column >= ArmyLayout.Columns - 2)
                return false; // too close to right edge

            var ids = Enumerable.Range(column, ArmyLayout.Columns)
                .TakeWhile(column => column < ArmyLayout.Columns)
                .Select(column => armyLayout[column, row])
                .TakeWhile(unit => unit is StandardUnit { ChargeState: null, ColorId: var nextColor } && nextColor == currentColor)
                .Select(unit => ((StandardUnit)unit).Id)
                .ToArray();
            if (ids.Length < 3)
                return false;
            match = new ArmyLayoutMatch(ids, true);
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
            if (row >= ArmyLayout.Rows - 2)
                return false; // too close to back of layout

            var ids = Enumerable.Range(row, ArmyLayout.Rows)
                .TakeWhile(row => row < ArmyLayout.Rows)
                .Select(row => armyLayout[column, row])
                .TakeWhile(unit => unit is StandardUnit { ChargeState: null, ColorId: var nextColor } && nextColor == currentColor)
                .Select(unit => ((StandardUnit)unit).Id)
                .Take(3) // TODO - what do we do if there's a 4-unit formation?
                .ToArray();
            if (ids.Length < 3)
                return false;
            match = new ArmyLayoutMatch(ids, false);
            return true;
        }
    }

    public readonly struct ArmyLayoutMatch
    {
        public IReadOnlyList<string> UnitIds { get; }
        public bool IsWall { get; }

        public ArmyLayoutMatch(IReadOnlyList<string> ids, bool isWall)
        {
            this.UnitIds = ids;
            this.IsWall = isWall;
        }
    }
}