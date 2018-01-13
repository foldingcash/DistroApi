namespace StatsDownload.Email
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;

    public class EmailProvider : IEmailService
    {
        private readonly IEmailSettingsValidatorService emailSettingsValidatorService;

        private readonly IEmailSettingsService settingsService;

        public EmailProvider(
            IEmailSettingsService settingsService,
            IEmailSettingsValidatorService emailSettingsValidatorService)
        {
            if (settingsService == null)
            {
                throw new ArgumentNullException(nameof(settingsService));
            }

            if (emailSettingsValidatorService == null)
            {
                throw new ArgumentNullException(nameof(emailSettingsValidatorService));
            }

            this.settingsService = settingsService;
            this.emailSettingsValidatorService = emailSettingsValidatorService;
        }

        public EmailResult SendEmail(string subject, string body)
        {
            try
            {
                MailAddress fromAddress = NewMailAddress(
                    settingsService.GetFromAddress(),
                    settingsService.GetFromDisplayName());

                IEnumerable<string> receivers = ParseReceivers(settingsService.GetReceivers());

                foreach (string address in receivers)
                {
                    MailAddress toAddress = NewMailAddress(address);
                    SendMessage(fromAddress, toAddress, subject, body);
                }

                return NewEmailResult();
            }
            catch (Exception ex)
            {
                return NewEmailResult(ex);
            }
        }

        private EmailResult NewEmailResult()
        {
            return new EmailResult();
        }

        private EmailResult NewEmailResult(Exception exception)
        {
            return new EmailResult(exception);
        }

        private MailAddress NewMailAddress(string address, string displayName)
        {
            return new MailAddress(address, displayName);
        }

        private MailAddress NewMailAddress(string address)
        {
            return new MailAddress(address);
        }

        private MailMessage NewMailMessage(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            return new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body };
        }

        private ICredentialsByHost NewNetworkCredential(string userName, string password)
        {
            return new NetworkCredential(userName, password);
        }

        private int ParsePort(string unsafePort)
        {
            return emailSettingsValidatorService.ParsePort(unsafePort);
        }

        private IEnumerable<string> ParseReceivers(string unsafeReceivers)
        {
            return emailSettingsValidatorService.ParseReceivers(unsafeReceivers);
        }

        private void SendMessage(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            int port = ParsePort(settingsService.GetPort());

            using (
                var smtpClient = new SmtpClient
                                     {
                                         Host = settingsService.GetSmtpHost(), Port = port, EnableSsl = true,
                                         DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false,
                                         Credentials =
                                             NewNetworkCredential(fromAddress.Address, settingsService.GetPassword())
                                     })
            {
                using (MailMessage mailMessage = NewMailMessage(fromAddress, toAddress, subject, body))
                {
                    smtpClient.Send(mailMessage);
                }
            }
        }
    }
}