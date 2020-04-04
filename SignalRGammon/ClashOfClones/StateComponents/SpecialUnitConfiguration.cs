using System;

namespace SignalRGammon.Clash
{
    namespace StateComponents
    {
        public readonly struct SpecialUnitConfiguration
        {
            public string UnitId { get; }
            public int Count { get; }

            public SpecialUnitConfiguration(string unitId, int count)
            {
                this.UnitId = unitId;
                this.Count = count;
            }
        }
    }
}
