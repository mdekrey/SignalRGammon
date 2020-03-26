namespace SignalRGammon.GameUtilities
{
    public interface IGameFactory
    {
        IGame CreateGame(string gameType);
    }
}