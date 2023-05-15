namespace rolling.dice.fun.Data
{
    /// <summary>
    /// user entropy item
    /// </summary>
    public class UserEntropy
    {
        /// <summary>
        /// username
        /// </summary>
        public string user { get; set; }
        /// <summary>
        /// current entropy
        /// </summary>
        public string entropy { get; set; }
        /// <summary>
        /// when added entropy
        /// </summary>
        public DateTime added_at { get; set; }
    }
}
