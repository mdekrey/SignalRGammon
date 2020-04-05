using System.Collections.Generic;

namespace SignalRGame.Checkers
{
    public static class PlayerExtensions
    {
        public static Player OtherPlayer(this Player p)
        {
            return p == Player.White ? Player.Black : Player.White;
        }
    }

}