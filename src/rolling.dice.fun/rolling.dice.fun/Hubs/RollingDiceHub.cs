using Microsoft.AspNetCore.SignalR;
using rolling.dice.fun.Data;

namespace rolling.dice.fun.Hubs
{
    public class RollingDiceHub : Hub
    {
        private readonly RollingDiceService rollingDiceService;

        public RollingDiceHub(RollingDiceService rollingDiceService)
        {
            this.rollingDiceService = rollingDiceService;
        }
        public async Task SendMessage(string user, string message)
        {
            rollingDiceService.AddEntropy(user, message);
            rollingDiceService.UpdateUser(Context.ConnectionId, user);
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            await UpdateStats();
        }

        async Task UpdateStats()
        {
            foreach (var user in RollingDiceService.Users)
            {
                var connectioUd = user.Key;
                var username = user.Value;
                var stats = rollingDiceService.GetUserStats(username);
                await Clients.Client(connectioUd).SendAsync("UpdateStatistics", stats.TotalSize, stats.UserSize, stats.UserPrc);
            }
        }
    }
}
