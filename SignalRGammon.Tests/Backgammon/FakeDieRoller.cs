using SignalRGammon.GameUtilities;

namespace SignalRGammon.Backgammon
{
    internal class FakeDieRoller : IDieRoller
    {
        private int rollIndex = 0;
        private readonly int[] dieRolls;

        public FakeDieRoller(params int[] dieRolls)
        {
            this.dieRolls = dieRolls;
        }

        public int RollDie(int sides = 6)
        {
            return dieRolls[rollIndex++];
        }
    }
}