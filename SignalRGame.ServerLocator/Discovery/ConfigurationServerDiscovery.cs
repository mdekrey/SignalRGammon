using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRGame.Discovery
{
    public class ConfigurationServerDiscovery : IServerDiscovery
    {
        private readonly IReadOnlyList<string> gameServers;

        public ConfigurationServerDiscovery(IReadOnlyList<string> gameServers)
        {
            this.gameServers = gameServers;
        }

        public IEnumerable<string> GetGameServers() => gameServers;
    }
}
