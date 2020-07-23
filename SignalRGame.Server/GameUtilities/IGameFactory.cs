using System.Linq;

namespace SignalRGame.GameUtilities
{
    public interface IGameFactory
    {
        IGame CreateGame(string gameType);
    }
}