namespace StatsDownload.Logging
{
    using System;
    using Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces;

    public class LoggingProvider : ILoggingService
    {
        private readonly IApplicationLoggingService applicationLoggingService;

        private readonly IDateTimeService dateTimeService;

        public LoggingProvider(IApplicationLoggingService applicationLoggingService, IDateTimeService dateTimeService)
        {
            if (applicationLoggingService == null)
            {
                throw new ArgumentNullException(nameof(applicationLoggingService));
            }

            if (dateTimeService == null)
            {
                throw new ArgumentNullException(nameof(dateTimeService));
            }

            this.applicationLoggingService = applicationLoggingService;
            this.dateTimeService = dateTimeService;
        }

        public void LogError(string message)
        {
            string messageWithTime = GetMessageWithTimestamp(message);

            applicationLoggingService.LogError(messageWithTime);
        }

        public void LogException(Exception exception)
        {
            LogError($"Exception Type: {exception.GetType()}{Environment.NewLine}"
                     + $"Exception Message: {exception.Message}{Environment.NewLine}"
                     + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        public void LogVerbose(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                string messageWithTime = GetMessageWithTimestamp(message);

                applicationLoggingService.LogVerbose(messageWithTime);
            }
        }

        private string GetMessageWithTimestamp(string message)
        {
            return dateTimeService.DateTimeNow() + Environment.NewLine + message;
        }
    }
}