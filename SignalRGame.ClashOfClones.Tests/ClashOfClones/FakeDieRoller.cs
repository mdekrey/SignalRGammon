using SignalRGame.GameUtilities;
using System;
using System.Linq;

namespace SignalRGame.ClashOfClones
{
    internal class FakeDieRoller : IDieRoller
    {
        private int rollIndex = 0;
        private readonly Func<int, int>[] dieRolls;

        public FakeDieRoller(params int[] dieRolls)
        {
            this.dieRolls = dieRolls.Select(roll => (Func<int, int>)(_ => roll)).ToArray();
        }

        public FakeDieRoller(params Func<int, int>[] dieRolls)
        {
            this.dieRolls = dieRolls;
        }

        public int RollDie(int sides = 6)
        {
            return dieRolls[rollIndex++](sides);
        }
    }
}