namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogFailedUserData(int downloadId, FailedUserData failedUserData);

        void LogResult(StatsUploadResult statsUploadResult);

        void LogResults(StatsUploadResults statsUploadResults);
    }
}