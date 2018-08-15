namespace StatsDownloadApi.WebApi
{
    using System;
    using Interfaces;
    using StatsDownload.Email;

    public class StatsDownloadApiEmailProvider : IStatsDownloadApiEmailService
    {
        private readonly IEmailService emailService;

        public StatsDownloadApiEmailProvider(IEmailService emailService)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public void SendUnhandledExceptionEmail(Exception exception)
        {
            emailService.SendEmail("API Unhandled Exception Caught",
                $"The StatsDownload API experienced an unhandled exception. Contact your technical advisor with the exception message. Exception Message: {exception.Message}.");
        }
    }
}