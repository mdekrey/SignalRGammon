namespace SignalRGammon
{
    public interface IGameFactory
    {
        IGame CreateGame(string gameType);
    }
}