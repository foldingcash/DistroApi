namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;
    using Logging;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}