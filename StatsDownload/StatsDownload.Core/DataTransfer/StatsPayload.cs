namespace StatsDownload.Core
{
    public class StatsPayload
    {
        public StatsPayload(int downloadId, string downloadUrl, string downloadTimeoutSeconds, string downloadFileName)
        {
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            DownloadTimeoutSeconds = downloadTimeoutSeconds;
            DownloadFileName = downloadFileName;
        }

        public string DownloadFileName { get; private set; }

        public int DownloadId { get; private set; }

        public string DownloadTimeoutSeconds { get; private set; }

        public string DownloadUrl { get; private set; }
    }
}