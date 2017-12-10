namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult()
        {
            Success = true;
        }

        public FileDownloadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}