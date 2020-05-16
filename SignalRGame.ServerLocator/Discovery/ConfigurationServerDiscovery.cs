using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Discovery
{
    public class ConfigurationServerDiscovery : IServerDiscovery
    {
        private readonly IReadOnlyList<ServerDetails> gameServers;

        public ConfigurationServerDiscovery(IReadOnlyList<ServerDetails> gameServers)
        {
            this.gameServers = gameServers;
        }

        public IEnumerable<ServerDetails> GetGameServers() => gameServers;

    }
}
