using System;
using System.Collections.Generic;

namespace SignalRGame.ClashOfClones
{
    namespace StateComponents
    {
        public readonly struct ArmyConfiguration
        {
            public const int ColorCount = StandardUnitConfiguration<string>.ColorCount;
            public const int MaxSpecialUnits = 5;

            public int MaxUnitCount { get; }
            public StandardUnitConfiguration<string> StandardUnits { get; }
            public IReadOnlyList<SpecialUnitConfiguration> SpecialUnits { get; }
            public ArmyConfiguration(int MaxUnitCount, StandardUnitConfiguration<string> StandardUnits, IReadOnlyList<SpecialUnitConfiguration> SpecialUnits)
            {
                if (SpecialUnits == null)
                    throw new ArgumentNullException(nameof(SpecialUnits));
                if (SpecialUnits.Count > MaxSpecialUnits)
                    throw new ArgumentException($"No more than {MaxSpecialUnits} special units", nameof(SpecialUnits));

                this.MaxUnitCount = MaxUnitCount;
                this.StandardUnits = StandardUnits;
                this.SpecialUnits = SpecialUnits;
            }
        }
    }
}
