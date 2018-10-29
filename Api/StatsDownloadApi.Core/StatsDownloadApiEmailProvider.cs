namespace StatsDownloadApi.Core
{
    using System;
    using Interfaces;
    using StatsDownload.Email;

    public class StatsDownloadApiEmailProvider : IStatsDownloadApiEmailService
    {
        private readonly IEmailService emailService;

        private readonly IEmailSettingsService emailSettingsService;

        public StatsDownloadApiEmailProvider(IEmailService emailService, IEmailSettingsService emailSettingsService)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.emailSettingsService =
                emailSettingsService ?? throw new ArgumentNullException(nameof(emailSettingsService));
        }

        public void SendUnhandledExceptionEmail(Exception exception)
        {
            string subject = GetSubject(EmailMessages.UnhandledExceptionHeader);

            emailService.SendEmail(subject,
                string.Format(EmailMessages.UnhandledExceptionBody, exception.Message));
        }

        private string GetSubject(string baseSubject)
        {
            string displayName = emailSettingsService.GetFromDisplayName();

            if (string.IsNullOrWhiteSpace(displayName))
            {
                return baseSubject;
            }

            return $"{displayName} - {baseSubject}";
        }
    }
}