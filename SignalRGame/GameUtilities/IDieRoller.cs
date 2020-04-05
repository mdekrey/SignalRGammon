using System;

namespace SignalRGame.GameUtilities
{
    public interface IDieRoller
    {
        int RollDie(int sides = 6);
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

        public int RollDie(int sides = 6)
        {
            return random.Next(1, sides + 1);
        }
    }
}