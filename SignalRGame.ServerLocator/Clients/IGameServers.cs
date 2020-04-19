using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Clients
{
    public interface IGameServers
    {
        Task<string?> FindGameServerAsync(string id, string type);
        Task<GameDetails?> CreateGameAsync(string gameType);
        Task<IReadOnlyList<string>> GetAllGameTypesAsync();
    }
}
