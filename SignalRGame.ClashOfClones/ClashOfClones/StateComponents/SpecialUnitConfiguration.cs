using System;

namespace SignalRGame.ClashOfClones
{
    namespace StateComponents
    {
        public readonly struct SpecialUnitConfiguration
        {
            public string UnitId { get; }
            public int Stock { get; }

            public SpecialUnitConfiguration(string unitId, int stock)
            {
                this.UnitId = unitId;
                this.Stock = stock;
            }
        }
    }
}
