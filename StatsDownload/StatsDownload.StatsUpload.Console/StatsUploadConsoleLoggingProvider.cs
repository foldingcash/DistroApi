namespace StatsDownload.StatsUpload.Console
{
    using System;
    using Core.Interfaces.Logging;
    using NLog;

    public class StatsUploadConsoleLoggingProvider : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public StatsUploadConsoleLoggingProvider(ILogger logger)
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