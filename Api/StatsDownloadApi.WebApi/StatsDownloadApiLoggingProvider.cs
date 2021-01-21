namespace StatsDownloadApi.WebApi
{
    using System;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces.Logging;

    public class StatsDownloadApiLoggingProvider : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public StatsDownloadApiLoggingProvider(ILogger<StatsDownloadApiLoggingProvider> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LogError(string message)
        {
            logger.LogError(message);
        }

        public void LogVerbose(string message)
        {
            logger.LogTrace(message);
        }
    }
}