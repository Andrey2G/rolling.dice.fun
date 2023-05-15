using Microsoft.AspNetCore.SignalR;

namespace rolling.dice.fun.Hubs
{
    public class RollingDiceHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
