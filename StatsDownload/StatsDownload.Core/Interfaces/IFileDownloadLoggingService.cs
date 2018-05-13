namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;
    using StatsDownload.Logging;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}