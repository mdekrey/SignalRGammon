using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SignalRGame.ClashOfClones.Rules
{
    public static class ApplyMatchLogic
    {
        public static ArmyLayout ApplyMatches(ArmyLayout layout, IEnumerable<ArmyLayoutMatch> matches, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            var units = layout.Units.ToImmutableList().ToBuilder();
            var ids = new HashSet<string>();
            foreach (var match in matches.OrderBy(m => m.IsWall ? 1 : 0)) // walls last!
            {
                switch (match)
                {
                    case ArmyLayoutMatch { IsWall: true }:
                        for (var col = 0; col < match.UnitIds.Count; col++)
                        {
                            ids.Add(match.UnitIds[col]);

                            if (units[ArmyLayout.GetIndexFor(match.Column + col, match.Row)] switch
                                {
                                    StandardUnit { Id: var id, ChargeState: null } when id == match.UnitIds[col] => false, // replace this directly
                                    EmptyPlaceholder _ => false, // replace this directly
                                    _ => true // better shift it all back to make room for the wall
                                })
                            {
                                // handle when this became part of something else
                                for (var r = ArmyLayout.Rows - 1; r > match.Row; r--)
                                {
                                    units[ArmyLayout.GetIndexFor(match.Column + col, r)] = units[ArmyLayout.GetIndexFor(match.Column + col, r - 1)];
                                }
                            }

                            units[ArmyLayout.GetIndexFor(match.Column + col, match.Row)] =
                                MakeWall(match.UnitIds[col], armyConfiguration, gameSettings);
                        }
                        break;
                    case ArmyLayoutMatch { IsWall: false }:
                        var nextId = match.UnitIds.Except(ids).First();
                        switch (layout[match.Column, match.Row])
                        {
                            case StandardUnit { ColorId: var colorId }:
                                units[ArmyLayout.GetIndexFor(match.Column, match.Row)] = MakeChargedStandardUnit(nextId, colorId, armyConfiguration, gameSettings);
                                units[ArmyLayout.GetIndexFor(match.Column, match.Row + 1)] = new UnitPart(nextId);
                                units[ArmyLayout.GetIndexFor(match.Column, match.Row + 2)] = new UnitPart(nextId);
                                break;
                            case EliteUnit { ColorId: var colorId, ArmySpecialUnitIndex: var specialUnitIndex }:
                                units[ArmyLayout.GetIndexFor(match.Column, match.Row)] = MakeChargedEliteUnit(nextId, colorId, specialUnitIndex, armyConfiguration, gameSettings);
                                if (units[ArmyLayout.GetIndexFor(match.Column, match.Row + 2)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column, match.Row + 2)] = new EmptyPlaceholder();
                                if (units[ArmyLayout.GetIndexFor(match.Column, match.Row + 3)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column, match.Row + 3)] = new EmptyPlaceholder();
                                break;
                            case ChampionUnit { ColorId: var colorId, ArmySpecialUnitIndex: var specialUnitIndex }:
                                units[ArmyLayout.GetIndexFor(match.Column, match.Row)] = MakeChargedChampionUnit(nextId, colorId, specialUnitIndex, armyConfiguration, gameSettings);
                                if (units[ArmyLayout.GetIndexFor(match.Column, match.Row + 2)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column, match.Row + 2)] = new EmptyPlaceholder();
                                if (units[ArmyLayout.GetIndexFor(match.Column, match.Row + 3)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column, match.Row + 3)] = new EmptyPlaceholder();
                                if (units[ArmyLayout.GetIndexFor(match.Column + 1, match.Row + 2)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column + 1, match.Row + 2)] = new EmptyPlaceholder();
                                if (units[ArmyLayout.GetIndexFor(match.Column + 1, match.Row + 3)] is StandardUnit)
                                    units[ArmyLayout.GetIndexFor(match.Column + 1, match.Row + 3)] = new EmptyPlaceholder();
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                }
            }

            return new ArmyLayout(units.ToImmutable());
        }

        private static UnitPlaceholder MakeChargedStandardUnit(string id, int colorId, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            var unit = gameSettings.Units[armyConfiguration.StandardUnits[colorId]];
            // TODO - apply rules
            return new StandardUnit(id, colorId, new ChargeState(1, unit.Attack - unit.ChargePerTurn * unit.ChargeTime, unit.ChargeTime));
        }

        private static UnitPlaceholder MakeChargedEliteUnit(string id, int colorId, int specialUnitIndex, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            var unit = gameSettings.Elites[armyConfiguration.SpecialUnits[specialUnitIndex].UnitId];
            // TODO - apply rules
            return new EliteUnit(id, colorId, new ChargeState(1, unit.Attack - unit.ChargePerTurn * unit.ChargeTime, unit.ChargeTime), specialUnitIndex);
        }

        private static UnitPlaceholder MakeChargedChampionUnit(string id, int colorId, int specialUnitIndex, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            var unit = gameSettings.Champions[armyConfiguration.SpecialUnits[specialUnitIndex].UnitId];
            // TODO - apply rules
            return new ChampionUnit(id, colorId, new ChargeState(1, unit.Attack - unit.ChargePerTurn * unit.ChargeTime, unit.ChargeTime), specialUnitIndex);
        }

        private static UnitPlaceholder MakeWall(string id, ArmyConfiguration armyConfiguration, GameSettings gameSettings)
        {
            return new WallUnit(id, 0 /* TODO - strength */);
        }
    }
}
