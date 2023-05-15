using System.Collections.Concurrent;
using System.Text;

namespace rolling.dice.fun.Data
{
    public class RollingDiceService
    {
        static ConcurrentQueue<UserEntropy> userEntropies = new ConcurrentQueue<UserEntropy>();
        public static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();
        static int nUsers = 0;

        public void AddEntropy(string user, string entropy)
        {
            userEntropies.Enqueue(new UserEntropy() { added_at = DateTime.UtcNow, entropy = entropy, user = user });
            Console.WriteLine($"{user}:{entropy}");
        }

        public Task<UserEntropy[]> GetEntropy()
        {
            return Task.FromResult(userEntropies.AsEnumerable().ToArray());
        }

        public void UpdateUser(string connectionId, string user)
        {
            if (!Users.TryAdd(connectionId,user))
                Users.TryUpdate(connectionId, user, Users[connectionId]);
        }

        public string GenerateUsername()
        {
            nUsers++;
            return $"Anonymous {nUsers}";
        }

        public StatsModel GetUserStats(string currentUser)
        {
            var result = new StatsModel();
            string entropy = "";
            foreach (var item in userEntropies)
            {
                entropy += item.entropy;

                if (item.user==currentUser) result.UserSize += UTF8Encoding.UTF8.GetByteCount(item.entropy);
            }
            result.TotalSize= UTF8Encoding.UTF8.GetByteCount(entropy);
            return result;
        }

        public List<string> GetUsers=> Users.Values.ToList();
    }
}