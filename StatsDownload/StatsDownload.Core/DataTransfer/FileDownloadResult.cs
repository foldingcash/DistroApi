namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult(StatsPayload statsPayload)
        {
            Success = true;
            StatsPayload = statsPayload;
        }

        public FileDownloadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public StatsPayload StatsPayload { get; private set; }

        public bool Success { get; private set; }
    }
}