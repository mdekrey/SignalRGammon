using System.Collections.Generic;

namespace SignalRGame.ClashOfClones
{
    public static class PlayerExtensions
    {
        public static Player OtherPlayer(this Player p)
        {
            return p == Player.White ? Player.Black : Player.White;
        }
    }

}