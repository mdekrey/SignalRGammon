using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Discovery
{
    public interface IServerDiscovery
    {
        IEnumerable<ServerDetails> GetGameServers();
    }
}
