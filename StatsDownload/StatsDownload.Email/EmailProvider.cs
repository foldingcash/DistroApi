namespace StatsDownload.Email
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;

    using StatsDownload.Logging;

    public class EmailProvider : IEmailService
    {
        private readonly IEmailSettingsValidatorService emailSettingsValidatorService;

        private readonly ILoggingService loggingService;

        private readonly IEmailSettingsService settingsService;

        private readonly int WaitTimeInMillisecondsBeforeTryingSendAgain = 5000;

        public EmailProvider(IEmailSettingsService settingsService,
                             IEmailSettingsValidatorService emailSettingsValidatorService,
                             ILoggingService loggingService)
        {
            if (settingsService == null)
            {
                throw new ArgumentNullException(nameof(settingsService));
            }

            if (emailSettingsValidatorService == null)
            {
                throw new ArgumentNullException(nameof(emailSettingsValidatorService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            this.settingsService = settingsService;
            this.emailSettingsValidatorService = emailSettingsValidatorService;
            this.loggingService = loggingService;
        }

        public EmailResult SendEmail(string subject, string body)
        {
            var sb = new StringBuilder();

            try
            {
                MailAddress fromAddress = NewMailAddress();

                IEnumerable<string> receivers = ParseReceivers(settingsService.GetReceivers());

                using (SmtpClient smtpClient = NewSmtpClient(sb, fromAddress))
                {
                    sb.AppendLine("Sending email:");
                    sb.AppendLine($"Subject: {subject}");
                    sb.AppendLine($"Body: {body}");
                    sb.AppendLine();

                    foreach (string address in receivers)
                    {
                        MailAddress toAddress = NewMailAddress(address);
                        SendMessage(sb, smtpClient, fromAddress, toAddress, subject, body);
                    }
                }

                LogVerbose(sb);
                return NewEmailResult();
            }
            catch (EmailArgumentException emailArgumentException)
            {
                LogVerbose(sb);
                LogException(emailArgumentException);
                LogEmailArgumentExceptionVerbose(emailArgumentException);
                return NewEmailResult(emailArgumentException);
            }
            catch (Exception ex)
            {
                LogVerbose(sb);
                LogException(ex);
                return NewEmailResult(ex);
            }
        }

        private void LogEmailArgumentExceptionVerbose(EmailArgumentException emailArgumentException)
        {
            var builder = new StringBuilder();

            builder.AppendLine("There was a problem with the email settings.");
            builder.AppendLine("Ensure all of the email settings are configured and valid before trying again.");
            builder.AppendLine();
            builder.AppendLine($"Exception message: {emailArgumentException.Message}");

            LogVerbose(builder);
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogVerbose(StringBuilder sb)
        {
            loggingService.LogVerbose(sb.ToString());
        }

        private EmailResult NewEmailResult()
        {
            return new EmailResult();
        }

        private EmailResult NewEmailResult(Exception exception)
        {
            return new EmailResult(exception);
        }

        private MailAddress NewMailAddress()
        {
            string fromAddress = ParseFromAddress(settingsService.GetFromAddress());
            string displayName = ParseFromDisplayName(settingsService.GetFromDisplayName());

            return NewMailAddress(fromAddress, displayName);
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

        private SmtpClient NewSmtpClient(StringBuilder sb, MailAddress fromAddress)
        {
            string host = ParseSmtpHost(settingsService.GetSmtpHost());
            int port = ParsePort(settingsService.GetPort());
            string password = ParsePassword(settingsService.GetPassword());
            ICredentialsByHost credentials = NewNetworkCredential(fromAddress.Address, password);
            var deliveryMethod = SmtpDeliveryMethod.Network;
            var enableSsl = true;
            var useDefaultCredentials = false;

            sb.AppendLine("Preparing Smtp client:");
            sb.AppendLine($"Host: {host}");
            sb.AppendLine($"Port: {port}");
            sb.AppendLine($"From Address: {fromAddress}");
            sb.AppendLine($"Delivery Method: {deliveryMethod}");
            sb.AppendLine($"Enable Ssl: {enableSsl}");
            sb.AppendLine($"Use Default Credentials: {useDefaultCredentials}");
            sb.AppendLine();

            return new SmtpClient
                   {
                       Host = host,
                       Port = port,
                       EnableSsl = enableSsl,
                       DeliveryMethod = deliveryMethod,
                       UseDefaultCredentials = useDefaultCredentials,
                       Credentials = credentials
                   };
        }

        private string ParseFromAddress(string unsafeFromAddress)
        {
            return emailSettingsValidatorService.ParseFromAddress(unsafeFromAddress);
        }

        private string ParseFromDisplayName(string unsafeFromDisplayName)
        {
            return emailSettingsValidatorService.ParseFromDisplayName(unsafeFromDisplayName);
        }

        private string ParsePassword(string unsafePassword)
        {
            return emailSettingsValidatorService.ParsePassword(unsafePassword);
        }

        private int ParsePort(string unsafePort)
        {
            return emailSettingsValidatorService.ParsePort(unsafePort);
        }

        private IEnumerable<string> ParseReceivers(string unsafeReceivers)
        {
            return emailSettingsValidatorService.ParseReceivers(unsafeReceivers);
        }

        private string ParseSmtpHost(string unsafeSmtpHost)
        {
            return emailSettingsValidatorService.ParseSmtpHost(unsafeSmtpHost);
        }

        private void SendMessage(StringBuilder sb, SmtpClient smtpClient, MailAddress fromAddress, MailAddress toAddress,
                                 string subject, string body)
        {
            sb.AppendLine($"Sending message to {toAddress.Address}");

            using (MailMessage mailMessage = NewMailMessage(fromAddress, toAddress, subject, body))
            {
                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (SmtpException smtpException)
                {
                    sb.AppendLine($"An smtp exception was caught with a status code of {smtpException.StatusCode}");

                    SmtpStatusCode statusCode = smtpException.StatusCode;
                    if (statusCode == SmtpStatusCode.MailboxBusy || statusCode == SmtpStatusCode.MailboxUnavailable)
                    {
                        sb.AppendLine($"Attempting to resend message to {toAddress.Address}");

                        Thread.Sleep(WaitTimeInMillisecondsBeforeTryingSendAgain);
                        smtpClient.Send(mailMessage);
                    }
                }
            }
        }
    }
}