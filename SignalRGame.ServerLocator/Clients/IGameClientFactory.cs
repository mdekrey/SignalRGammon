using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Clients
{
    public interface IGameClientFactory
    {
        Clients.IGameApiClient CreateGameApiClient(string url);
    }
}
