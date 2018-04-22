namespace StatsDownload.Core
{
    using Interfaces;
    using StatsDownload.Logging;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}