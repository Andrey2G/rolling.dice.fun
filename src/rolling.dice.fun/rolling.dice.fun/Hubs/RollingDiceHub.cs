using Microsoft.AspNetCore.SignalR;
using rolling.dice.fun.Data;

namespace rolling.dice.fun.Hubs
{
    /// <summary>
    /// SignalR Hub for Rolling Dice realtime session logic
    /// </summary>
    public class RollingDiceHub : Hub
    {
        /// <summary>
        /// instance of the Rolling Dice service for managing users, their entropies and calculating the result
        /// </summary>
        private readonly RollingDiceService rollingDiceService;

        public RollingDiceHub(RollingDiceService rollingDiceService)
        {
            this.rollingDiceService = rollingDiceService;
        }
        /// <summary>
        /// User click on Add Entropy
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="message">entropy</param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            rollingDiceService.AddEntropy(user, message);
            rollingDiceService.UpdateUser(Context.ConnectionId, user);
            //need to notify the others users about new entropy
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            //update the stats
            await UpdateStats();
        }

        /// <summary>
        /// Stop the Count!
        /// disable the logic of adding new entropy and calculate the result
        /// </summary>
        /// <returns></returns>
        public async Task StopTheCount()
        {
            //notify all users about that session is completed
            await Clients.All.SendAsync("StoppingTheCount");
            //calculating rolling dice result
            var result = rollingDiceService.CalculateEntropy();
            //notify all users with calculated result
            await Clients.All.SendAsync("RollingDiceResult",result);
        }

        /// <summary>
        /// update the stats for each user
        /// </summary>
        /// <returns></returns>
        async Task UpdateStats()
        {
            foreach (var user in RollingDiceService.Users)
            {
                var connectionId = user.Key;
                var username = user.Value;
                var stats = rollingDiceService.GetUserStats(username);
                await Clients.Client(connectionId).SendAsync("UpdateStatistics", stats.TotalSize, stats.UserSize, stats.UserPrc);
            }
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            rollingDiceService.RemoveUserConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
