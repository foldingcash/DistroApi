namespace StatsDownload.Core
{
    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);
    }
}