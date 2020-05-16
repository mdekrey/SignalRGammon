using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Discovery
{
    public interface IGameServers
    {
        Task<ServerDetails?> FindGameServerAsync(string id, string type);

        Task<GameDetails?> CreateGameAsync(string gameType);
        // TODO - this should not return a controller type
        Task<IReadOnlyList<Controllers.GameType>> GetAllGameTypesAsync();
    }
}
