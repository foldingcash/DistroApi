namespace StatsDownload.Core
{
    public class StatsPayload
    {
        public string DownloadFilePath { get; set; }

        public int DownloadId { get; set; }

        public string DownloadUrl { get; set; }

        public string StatsData { get; set; }

        public int TimeoutSeconds { get; set; }

        public string UncompressedDownloadFilePath { get; set; }
    }
}