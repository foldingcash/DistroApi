namespace StatsDownload.Core
{
    public class StatsUploadResult
    {
        public StatsUploadResult()
        {
            Success = true;
        }

        public StatsUploadResult(int downloadId, FailedReason failedReason)
        {
            Success = false;
            DownloadId = downloadId;
            FailedReason = failedReason;
        }

        public int DownloadId { get; private set; }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}