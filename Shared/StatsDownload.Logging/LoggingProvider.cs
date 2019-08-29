namespace StatsDownload.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;

    public class LoggingProvider : ILoggingService
    {
        private readonly IApplicationLoggingService applicationLoggingService;

        private readonly IDateTimeService dateTimeService;

        public LoggingProvider(IApplicationLoggingService applicationLoggingService, IDateTimeService dateTimeService)
        {
            this.applicationLoggingService = applicationLoggingService
                                             ?? throw new ArgumentNullException(nameof(applicationLoggingService));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
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

        public void LogMethodFinished([CallerMemberName] string method = "")
        {
            LogVerbose($"{method} Finished");
        }

        public void LogMethodInvoked([CallerMemberName] string method = "")
        {
            LogVerbose($"{method} Invoked");
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