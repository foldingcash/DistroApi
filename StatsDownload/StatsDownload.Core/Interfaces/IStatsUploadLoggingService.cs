namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;
    using Logging;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogFailedUserData(int downloadId, FailedUserData failedUserData);

        void LogResult(StatsUploadResult statsUploadResult);

        void LogResults(StatsUploadResults statsUploadResults);
    }
}