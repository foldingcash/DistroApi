namespace StatsDownloadApi.WebApi
{
    using System;

    using NLog;

    using StatsDownload.Core.Interfaces.Logging;

    public class StatsDownloadApiLoggingProvider : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public StatsDownloadApiLoggingProvider(ILogger logger)
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

        public void LogInformation(string message)
        {
            logger.Info(message);
        }
    }
}