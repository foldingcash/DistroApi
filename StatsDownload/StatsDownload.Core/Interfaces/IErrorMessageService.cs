namespace StatsDownload.Core
{
    using Interfaces;

    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);
    }
}