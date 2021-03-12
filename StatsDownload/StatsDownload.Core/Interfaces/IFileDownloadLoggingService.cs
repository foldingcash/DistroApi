namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileDownloadLoggingService
    {
        void LogResult(FileDownloadResult result);
    }
}