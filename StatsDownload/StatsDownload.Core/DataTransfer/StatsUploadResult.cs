namespace StatsDownload.Core
{
    public class StatsUploadResult
    {
        public StatsUploadResult()
        {
            Success = true;
        }

        public StatsUploadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}