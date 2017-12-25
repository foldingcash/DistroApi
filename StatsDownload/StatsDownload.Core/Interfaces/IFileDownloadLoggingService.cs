namespace StatsDownload.Core
{
    using System;

    public interface IFileDownloadLoggingService
    {
        void LogException(Exception exception);

        void LogResult(FileDownloadResult result);

        void LogVerbose(string message);
    }
}