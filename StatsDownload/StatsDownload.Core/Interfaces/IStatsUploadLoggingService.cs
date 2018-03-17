namespace StatsDownload.Core
{
    using StatsDownload.Logging;

    public interface IStatsUploadLoggingService : ILoggingService
    {
        void LogResult(StatsUploadResult statsUploadResult);
    }
}