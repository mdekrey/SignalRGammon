﻿using System.Collections.Generic;

namespace SignalRGammon.Backgammon
{
    public static class PlayerExtensions
    {
        public static Player OtherPlayer(this Player p)
        {
            return p == Player.White ? Player.Black : Player.White;
        }
    }

}