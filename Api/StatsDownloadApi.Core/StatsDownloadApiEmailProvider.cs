namespace StatsDownloadApi.Core
{
    using System;

    using Microsoft.Extensions.Options;

    using StatsDownload.Email;

    using StatsDownloadApi.Interfaces;

    public class StatsDownloadApiEmailProvider : IStatsDownloadApiEmailService
    {
        private readonly IEmailService emailService;

        private readonly EmailSettings emailSettings;

        public StatsDownloadApiEmailProvider(IEmailService emailService, IOptions<EmailSettings> emailSettings)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public void SendUnhandledExceptionEmail(Exception exception)
        {
            string subject = GetSubject(EmailMessages.UnhandledExceptionHeader);

            emailService.SendEmail(subject, string.Format(EmailMessages.UnhandledExceptionBody, exception.Message));
        }

        private string GetSubject(string baseSubject)
        {
            string displayName = emailSettings.DisplayName;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                return baseSubject;
            }

            return $"{displayName} - {baseSubject}";
        }
    }
}