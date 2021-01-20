namespace StatsDownload.Core.Interfaces.Settings
{
    public class DownloadSettings
    {
        public bool AcceptAnySslCert { get; set; }

        public string DownloadDirectory { get; set; }

        public int DownloadTimeout { get; set; }

        public string DownloadUri { get; set; }

        public int MinimumWaitTimeInHours { get; set; }
    }
}