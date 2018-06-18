namespace StatsDownload.Core.Interfaces.Logging
{
    using System;

    public interface ILoggingService : IApplicationLoggingService
    {
        void LogException(Exception exception);
    }
}