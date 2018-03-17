namespace StatsDownload.Core
{
    public class StatsUploadResults
    {
        public StatsUploadResults()
        {
            Success = true;
        }

        public StatsUploadResults(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}