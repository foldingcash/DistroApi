namespace StatsDownload.Core
{
    public class FileDownloadResult
    {
        public FileDownloadResult(FilePayload filePayload)
        {
            Success = true;
            FilePayload = filePayload;
        }

        public FileDownloadResult(FailedReason failedReason, FilePayload filePayload)
        {
            Success = false;
            FailedReason = failedReason;
            FilePayload = filePayload;
        }

        public FailedReason FailedReason { get; private set; } = FailedReason.None;

        public FilePayload FilePayload { get; private set; }

        public bool Success { get; private set; }
    }
}