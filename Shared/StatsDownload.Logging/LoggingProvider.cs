namespace StatsDownload.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;

    public class LoggingProvider : ILoggingService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILogger<LoggingProvider> logger;

        public LoggingProvider(ILogger<LoggingProvider> logger, IDateTimeService dateTimeService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
        }

        public void LogDebug(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                string messageWithTime = GetMessageWithTimestamp(message);

                logger.LogDebug(messageWithTime);
            }
        }

        public void LogError(string message)
        {
            string messageWithTime = GetMessageWithTimestamp(message);

            logger.LogError(messageWithTime);
        }

        public void LogException(Exception exception)
        {
            LogError($"Exception Type: {exception.GetType()}{Environment.NewLine}"
                     + $"Exception Message: {exception.Message}{Environment.NewLine}"
                     + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        public void LogMethodFinished([CallerMemberName] string method = "")
        {
            LogDebug($"{method} Finished");
        }

        public void LogMethodInvoked([CallerMemberName] string method = "")
        {
            LogDebug($"{method} Invoked");
        }

        private string GetMessageWithTimestamp(string message)
        {
            return dateTimeService.DateTimeNow() + Environment.NewLine + message;
        }
    }
}