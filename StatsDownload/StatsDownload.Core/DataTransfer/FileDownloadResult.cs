namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult(FilePayload filePayload)
            : this(true, FailedReason.None, filePayload)
        {
        }

        public FileDownloadResult(FailedReason failedReason, FilePayload filePayload)
            : this(false, failedReason, filePayload)
        {
        }

        private FileDownloadResult(bool success, FailedReason failedReason, FilePayload filePayload)
        {
            Success = success;
            FailedReason = failedReason;
            FilePayload = filePayload;
        }

        public FailedReason FailedReason { get; }

        public FilePayload FilePayload { get; }

        public bool Success { get; }
    }
}