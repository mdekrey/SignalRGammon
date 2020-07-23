namespace SignalRGame.GameUtilities
{
    public interface ISingleGameFactory
    {
        string Type { get; }
        IGame CreateGame();
    }
}