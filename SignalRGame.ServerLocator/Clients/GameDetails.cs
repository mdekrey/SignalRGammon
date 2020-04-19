namespace SignalRGame.Clients
{
    public readonly struct GameDetails
    {
        public string Id { get; }
        public string ServerUrl { get; }

        public GameDetails(string id, string serverUrl)
        {
            this.Id = id;
            this.ServerUrl = serverUrl;
        }
    }
}