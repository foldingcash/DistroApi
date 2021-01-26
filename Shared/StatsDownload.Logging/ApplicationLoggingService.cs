namespace StatsDownload.Logging
{
    using System;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces.Logging;

    public class ApplicationLoggingService : IApplicationLoggingService
    {
        private readonly ILogger logger;

        public ApplicationLoggingService(ILogger<ApplicationLoggingService> logger)
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