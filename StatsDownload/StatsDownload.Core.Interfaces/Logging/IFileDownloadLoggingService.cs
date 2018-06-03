namespace StatsDownload.Core.Interfaces.Logging
{
    using DataTransfer;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}