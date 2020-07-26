using System.Collections.Generic;

namespace SignalRGame.Backgammon
{
    public record PlayerState<T>
    {
        public T White { get; init; } = default!;
        public T Black { get; init; } = default!;

        public PlayerState()
        {
        }

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