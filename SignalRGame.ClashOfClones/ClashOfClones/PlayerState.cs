using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.ClashOfClones
{
    public readonly struct PlayerState<T>
    {
        public readonly T White;
        public readonly T Black;

        public PlayerState(T white, T black)
        {
            White = white;
            Black = black;
        }

        public PlayerState(Player player, T value, T otherValue) : this(otherValue, otherValue)
        {

            if (player == Player.White)
                White = value;
            else
                Black = value;
        }

        public T this[Player player] => player == Player.White ? White : Black;

        public PlayerState<T> With(Player player, T value) =>
            new PlayerState<T>(
                white: player == Player.White ? value : White,
                black: player == Player.Black ? value : Black
            );
    }
}
