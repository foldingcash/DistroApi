namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public interface IFileDownloadLoggingService : ILoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}