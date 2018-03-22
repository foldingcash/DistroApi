namespace StatsDownload.Core
{
    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);

        string GetErrorMessage(FailedReason failedReason);
    }
}