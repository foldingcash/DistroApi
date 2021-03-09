namespace StatsDownload.Core.Interfaces
{
    public interface IStatsDownloadLoggingService : IFileDownloadLoggingService, IStatsUploadLoggingService
    {
        void LogError(string message);
    }
}