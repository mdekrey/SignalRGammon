using SignalRGame.ClashOfClones.StateComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalRGame.ClashOfClones.Rules
{
    static class ArmyLayoutAssertionUtilities
    {
        public static Action<UnitPlaceholder>[] GetUnitAssertions(Action<(int column, int row), UnitPlaceholder> assertion)
        {
            return Enumerable.Range(0, ArmyLayout.TotalCount)
                             .Select(index => (Action<UnitPlaceholder>)(unit => assertion(ArmyLayout.GetPositionFor(index), unit)))
                             .ToArray();
        }

        public static string NewId() => Guid.NewGuid().ToString();

    }
}
