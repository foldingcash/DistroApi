namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public class StatsUploadResults
    {
        public StatsUploadResults(List<StatsUploadResult> uploadResults)
        {
            Success = true;
            UploadResults = uploadResults;
        }

        public StatsUploadResults(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }

        public List<StatsUploadResult> UploadResults { get; private set; }
    }
}