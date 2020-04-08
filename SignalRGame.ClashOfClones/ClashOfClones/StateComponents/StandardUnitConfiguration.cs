using System;

namespace SignalRGame.ClashOfClones
{
    namespace StateComponents
    {
        public readonly struct StandardUnitConfiguration<T>
        {
            public const int ColorCount = 3;

            public T First { get; }
            public T Second { get; }
            public T Third { get; }

            public StandardUnitConfiguration(T first, T second, T third)
            {
                this.First = first;
                this.Second = second;
                this.Third = third;
            }

            public StandardUnitConfiguration<T> Set(int index, T value) =>
                index switch
                {
                    1 => new StandardUnitConfiguration<T>(value, Second, Third),
                    2 => new StandardUnitConfiguration<T>(First, value, Third),
                    3 => new StandardUnitConfiguration<T>(First, Second, value),
                    _ => throw new IndexOutOfRangeException()
                };

            public T this[int index] =>
                index switch
                {
                    0 => First,
                    1 => Second,
                    2 => Third,
                    _ => throw new IndexOutOfRangeException()
                };
        }
    }
}
