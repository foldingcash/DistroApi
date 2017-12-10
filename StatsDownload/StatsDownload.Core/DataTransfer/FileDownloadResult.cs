namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult(int downloadId)
        {
            Success = true;
            DownloadId = downloadId;
        }

        public FileDownloadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public int DownloadId { get; set; }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}