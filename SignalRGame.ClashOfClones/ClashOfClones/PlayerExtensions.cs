using System.Collections.Generic;

namespace SignalRGammon.Clash
{
    public static class PlayerExtensions
    {
        public static Player OtherPlayer(this Player p)
        {
            return p == Player.White ? Player.Black : Player.White;
        }
    }

}