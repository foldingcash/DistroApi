namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IStatsUploadLoggingService
    {
        void LogFailedUserData(int downloadId, FailedUserData failedUserData);

        void LogResult(StatsUploadResult statsUploadResult);

        void LogResults(StatsUploadResults statsUploadResults);
    }
}