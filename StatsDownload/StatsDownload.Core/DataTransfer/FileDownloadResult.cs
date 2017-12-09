namespace StatsDownload.Core
{
    using System;

    public class FileDownloadResult
    {
        public FileDownloadResult()
        {
            Success = true;
        }

        public FileDownloadResult(Exception exception)
        {
            Success = false;
            Exception = exception;
        }

        public FileDownloadResult(FailedReason failedReason)
        {
            Success = false;
            FailedReason = failedReason;
        }

        public Exception Exception { get; private set; }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public bool Success { get; private set; }
    }
}