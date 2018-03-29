namespace StatsDownload.Logging
{
    using System;

    public class LoggingProvider : ILoggingService
    {
        private readonly IApplicationLoggingService applicationLoggingService;

        public LoggingProvider(IApplicationLoggingService applicationLoggingService)
        {
            if (applicationLoggingService == null)
            {
                throw new ArgumentNullException(nameof(applicationLoggingService));
            }

            this.applicationLoggingService = applicationLoggingService;
        }

        public void LogError(string message)
        {
            applicationLoggingService.LogError(message);
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
                applicationLoggingService.LogVerbose(message);
            }
        }
    }
}