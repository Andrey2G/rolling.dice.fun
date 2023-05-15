namespace rolling.dice.fun.Data
{
    public class RollingDiceService
    {
        public async Task AddEntropy(string user, string entropy)
        {
            Console.WriteLine($"{user}:{entropy}");
        }
    }
}