namespace StatsDownload.Core
{
    using StatsDownload.Logging;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogFailedUserData(FailedUserData failedUserData);

        void LogResult(StatsUploadResult statsUploadResult);

        void LogResults(StatsUploadResults statsUploadResults);
    }
}