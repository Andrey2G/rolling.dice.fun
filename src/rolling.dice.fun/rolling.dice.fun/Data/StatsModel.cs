namespace rolling.dice.fun.Data
{
    public class StatsModel
    {
        public int TotalSize { get; set; }
        public int UserSize { get; set; }
        public int UserPrc { get { return (int)(100 * (float)UserSize / (float)TotalSize); } }
    }
}
