﻿using SignalRGame.GameUtilities;

namespace SignalRGame.Backgammon
{
    internal class FakeDieRoller : IDieRoller
    {
        private int rollIndex = 0;
        private readonly int[] dieRolls;

        public FakeDieRoller(params int[] dieRolls)
        {
            this.dieRolls = dieRolls;
        }

        public int RollDie()
        {
            return dieRolls[rollIndex++];
        }
    }
}