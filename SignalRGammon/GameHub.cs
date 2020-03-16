using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRGammon
{
    public class GameHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
