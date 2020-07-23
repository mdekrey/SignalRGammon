using System;

namespace SignalRGame.Backgammon
{
    public interface IDieRoller
    {
        int RollDie();
    }

    public class DieRoller : IDieRoller
    {
        private readonly Random random;

        public DieRoller()
        {
            random = new Random();
        }

        public DieRoller(int seed)
        {
            random = new Random(seed);
        }

        public int RollDie()
        {
            return random.Next(0, 6) + 1;
        }
    }
}