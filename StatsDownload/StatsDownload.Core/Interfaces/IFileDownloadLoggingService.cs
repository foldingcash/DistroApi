namespace StatsDownload.Core
{
    using StatsDownload.Logging;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}