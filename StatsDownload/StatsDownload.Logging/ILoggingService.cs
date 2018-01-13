namespace StatsDownload.Logging
{
    using System;

    public interface ILoggingService
    {
        void LogException(Exception exception);

        void LogVerbose(string message);
    }
}