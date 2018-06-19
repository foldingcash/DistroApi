namespace StatsDownload.Core.Interfaces.DataTransfer
{
    using System;
    using Enums;

    public class StatsUploadResult
    {
        public StatsUploadResult()
            : this(true, 0, FailedReason.None, null)
        {
        }

        public StatsUploadResult(int downloadId, FailedReason failedReason)
            : this(false, downloadId, failedReason, null)
        {
        }

        public StatsUploadResult(int downloadId, FailedReason failedReason, Exception exception)
            : this(false, downloadId, failedReason, exception)
        {
        }

        private StatsUploadResult(bool success, int downloadId, FailedReason failedReason, Exception exception)
        {
            Success = success;
            DownloadId = downloadId;
            FailedReason = failedReason;
            Exception = exception;
        }

        public int DownloadId { get; }

        public Exception Exception { get; }

        public FailedReason FailedReason { get; }

        public bool Success { get; }
    }
}