using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalRGame.Clients
{
    public class GameClientFactory : IGameClientFactory
    {
        private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings();
        private readonly HttpClient httpClient;

        public GameClientFactory(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IGameApiClient CreateGameApiClient(string url) =>
            new GameApiClient(httpClient, new GameApiClientConfiguration
            {
                BaseUrl = url,
                Settings = JsonSettings,
            });
    }
}
