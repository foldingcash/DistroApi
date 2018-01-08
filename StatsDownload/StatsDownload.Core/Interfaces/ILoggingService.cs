namespace StatsDownload.Core
{
    using System;

    public interface ILoggingService
    {
        void LogException(Exception exception);

        void LogResult(FileDownloadResult result);

        void LogVerbose(string message);
    }
}