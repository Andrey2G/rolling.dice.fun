using System.Collections.Concurrent;
using System.Text;

namespace rolling.dice.fun.Data
{
    public class RollingDiceService
    {
        /// <summary>
        /// session prefix for storing states in localstorage
        /// </summary>
        static string SessionPrefix = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// collecting entropy for each user
        /// </summary>
        static ConcurrentQueue<UserEntropy> userEntropies = new ConcurrentQueue<UserEntropy>();
        /// <summary>
        /// collecting users
        /// </summary>
        public static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// number of users to generate unique anonymous users
        /// </summary>
        static int nUsers = 0;

        /// <summary>
        /// prepare state name by adding a session prefix to avoid mess in session values for each user
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string PrepareStateName(string state) => $"{RollingDiceService.SessionPrefix}_{state}";

        /// <summary>
        /// Add entropy value from specified user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entropy"></param>
        public void AddEntropy(string user, string entropy)
        {
            userEntropies.Enqueue(new UserEntropy() { added_at = DateTime.UtcNow, entropy = entropy, user = user });
            Console.WriteLine($"{user}:{entropy}");
        }

        /// <summary>
        /// get all messages added by users in current session
        /// </summary>
        /// <returns></returns>
        public Task<UserEntropy[]> GetEntropy()
        {
            return Task.FromResult(userEntropies.AsEnumerable().ToArray());
        }

        /// <summary>
        /// update anonymous username
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="user"></param>
        public void UpdateUser(string connectionId, string user)
        {
            if (!Users.TryAdd(connectionId, user))
                Users.TryUpdate(connectionId, user, Users[connectionId]);
        }

        /// <summary>
        /// remove connection from collection when user disconnected
        /// </summary>
        /// <param name="connectionId"></param>
        public void RemoveUserConnection(string connectionId)
        {
            Users.Remove(connectionId, out string username);
        }

        /// <summary>
        /// generate Anonymous username
        /// </summary>
        /// <returns></returns>
        public string GenerateUsername()
        {
            nUsers++;
            return $"Anonymous {nUsers}";
        }

        /// <summary>
        /// read user statistics on entropy
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public StatsModel GetUserStats(string currentUser)
        {
            var result = new StatsModel();
            string entropy = "";
            foreach (var item in userEntropies)
            {
                entropy += item.entropy;

                if (item.user == currentUser) result.UserSize += UTF8Encoding.UTF8.GetByteCount(item.entropy);
            }
            result.TotalSize = UTF8Encoding.UTF8.GetByteCount(entropy);
            return result;
        }


        /// <summary>
        /// Calculet Entropy - Rolling dice result!
        /// </summary>
        /// <returns></returns>
        public int CalculateEntropy()
        {
            List<byte> entropyBytes = new List<byte>();

            foreach(var item in userEntropies.ToList())
            {
                entropyBytes.AddRange(UTF8Encoding.UTF8.GetBytes(item.entropy));
            }

            var bigInteger = BitConverter.ToUInt32(entropyBytes.ToArray(), 0);
            var result = (int)(bigInteger % 6 + 1);
            return result;
        }
    }
}