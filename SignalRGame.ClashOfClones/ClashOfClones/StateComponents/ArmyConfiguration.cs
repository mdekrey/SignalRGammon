using System;
using System.Collections.Generic;

namespace SignalRGammon.Clash
{
    namespace StateComponents
    {
        public readonly struct ArmyConfiguration
        {
            public const int ColorCount = StandardUnitConfiguration<string>.ColorCount;
            public const int MaxSpecialUnits = 5;

            public StandardUnitConfiguration<string> StandardUnits { get; }
            public IReadOnlyList<SpecialUnitConfiguration> SpecialUnits { get; }
            public ArmyConfiguration(StandardUnitConfiguration<string> StandardUnits, IReadOnlyList<SpecialUnitConfiguration> SpecialUnits)
            {
                if (SpecialUnits == null)
                    throw new ArgumentNullException(nameof(SpecialUnits));
                if (SpecialUnits.Count > MaxSpecialUnits)
                    throw new ArgumentException($"No more than {MaxSpecialUnits} special units", nameof(SpecialUnits));

                this.StandardUnits = StandardUnits;
                this.SpecialUnits = SpecialUnits;
            }
        }
    }
}
