namespace StatsDownload.Core
{
    public class StatsPayload
    {
        public StatsPayload(
            int downloadId,
            string downloadUrl,
            int timeoutSeconds,
            string downloadFilePath,
            string uncompressedDownloadFilePath)
        {
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            TimeoutSeconds = timeoutSeconds;
            DownloadFilePath = downloadFilePath;
            UncompressedDownloadFilePath = uncompressedDownloadFilePath;
        }

        public string DownloadFilePath { get; private set; }

        public int DownloadId { get; private set; }

        public string DownloadUrl { get; private set; }

        public string StatsData { get; set; }

        public int TimeoutSeconds { get; private set; }

        public string UncompressedDownloadFilePath { get; set; }
    }
}