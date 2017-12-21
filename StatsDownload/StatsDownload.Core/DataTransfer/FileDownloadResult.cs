namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult(int downloadId, string downloadUrl, string downloadTimeout, string downloadDirectory)
        {
            Success = true;
            DownloadId = downloadId;
            DownloadUrl = downloadUrl;
            DownloadTimeout = downloadTimeout;
            DownloadDirectory = downloadDirectory;
        }

        public FileDownloadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public string DownloadDirectory { get; private set; }

        public int DownloadId { get; private set; }

        public string DownloadTimeout { get; private set; }

        public string DownloadUrl { get; private set; }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}