namespace SignalRGame.Discovery
{
    public readonly struct ServerDetails
    {
        public ServerDetails(string internalUrl, string publicUrl)
        {
            this.InternalUrl = internalUrl;
            this.PublicUrl = publicUrl;
        }

        public string InternalUrl { get; }
        public string PublicUrl { get; }
    }
}