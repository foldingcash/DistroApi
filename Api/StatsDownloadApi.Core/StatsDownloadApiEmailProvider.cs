namespace StatsDownloadApi.Core
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
            emailService.SendEmail(EmailMessages.UnhandledExceptionHeader,
                string.Format(EmailMessages.UnhandledExceptionBody, exception.Message));
        }
    }
}