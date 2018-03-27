namespace StatsDownload.Core
{
    public interface IStatsDownloadLoggingService : IFileDownloadLoggingService, IStatsUploadLoggingService
    {
    }
}