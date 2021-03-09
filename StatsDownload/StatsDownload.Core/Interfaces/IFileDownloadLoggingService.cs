namespace StatsDownload.Core.Interfaces
{
    using System;

    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileDownloadLoggingService
    {
        void LogDebug(string message);

        void LogException(Exception exception);

        void LogResult(FileDownloadResult result);
    }
}