namespace StatsDownload.Core
{
    public class StatsPayload
    {
        public StatsPayload(int downloadId, string downloadUrl, int timeoutSeconds, string downloadFileName)
        {
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            TimeoutSeconds = timeoutSeconds;
            DownloadFileName = downloadFileName;
        }

        public string DownloadFileName { get; private set; }

        public int DownloadId { get; private set; }

        public string DownloadUrl { get; private set; }

        public int TimeoutSeconds { get; private set; }
    }
}