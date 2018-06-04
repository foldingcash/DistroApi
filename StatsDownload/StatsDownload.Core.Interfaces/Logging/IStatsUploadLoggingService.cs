namespace StatsDownload.Core.Interfaces.Logging
{
    using DataTransfer;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogFailedUserData(int downloadId, FailedUserData failedUserData);

        void LogResult(StatsUploadResult statsUploadResult);

        void LogResults(StatsUploadResults statsUploadResults);
    }
}