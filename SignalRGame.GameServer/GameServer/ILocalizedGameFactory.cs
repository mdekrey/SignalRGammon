using SignalRGame.GameUtilities;

namespace SignalRGame.GameServer
{
    public interface ILocalizedGameFactory : ISingleGameFactory
    {
        string DisplayName { get; }
        string IconUrl { get; }
    }
}