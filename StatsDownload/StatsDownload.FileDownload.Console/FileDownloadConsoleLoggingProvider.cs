namespace StatsDownload.FileDownload.Console
{
    using System;

    using NLog;

    using StatsDownload.Core.Interfaces.Logging;

    public class FileDownloadConsoleLoggingProvider : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public FileDownloadConsoleLoggingProvider(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogError(string message)
        {
            logger.Error(message);
        }

        public void LogVerbose(string message)
        {
            logger.Trace(message);
        }
    }
}