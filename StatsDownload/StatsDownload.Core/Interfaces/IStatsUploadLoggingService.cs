namespace StatsDownload.Core
{
    using StatsDownload.Logging;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogException(FailedUserData failedUserData);

        void LogResult(StatsUploadResult result);
    }
}