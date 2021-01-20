namespace StatsDownload.Core.Interfaces
{
    public class DatabaseSettings
    {
        public int? CommandTimeout { get; set; }

        public string ConnectionString { get; set; }

        public string Type { get; set; }
    }
}