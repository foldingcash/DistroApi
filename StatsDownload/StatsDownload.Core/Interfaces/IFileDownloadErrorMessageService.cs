namespace StatsDownload.Core
{
    public interface IFileDownloadErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);
    }
}