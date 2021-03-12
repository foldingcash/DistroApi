namespace StatsDownload.Email
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class EmailProvider : IEmailService
    {
        private readonly EmailSettings emailSettings;

        private readonly IEmailSettingsValidatorService emailSettingsValidatorService;

        private readonly ILogger logger;

        private readonly int WaitTimeInMillisecondsBeforeTryingSendAgain = 5000;

        public EmailProvider(ILogger<EmailProvider> logger, IOptions<EmailSettings> emailSettings,
                             IEmailSettingsValidatorService emailSettingsValidatorService)
        {
            this.emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.emailSettingsValidatorService = emailSettingsValidatorService
                                                 ?? throw new ArgumentNullException(
                                                     nameof(emailSettingsValidatorService));
        }

        public EmailResult SendEmail(string subject, string body)
        {
            try
            {
                logger.LogInformation("Attempting to send email(s)");
                logger.LogDebug("Sending email(s) with subject {subject} and body {body}", subject, body);

                MailAddress fromAddress = NewMailAddress();
                IEnumerable<string> receivers = ParseReceivers(emailSettings.Receivers);

                using (SmtpClient smtpClient = NewSmtpClient(fromAddress))
                {
                    foreach (string address in receivers)
                    {
                        MailAddress toAddress = NewMailAddress(address);
                        SendMessage(smtpClient, fromAddress, toAddress, subject, body);
                    }
                }

                return NewEmailResult();
            }
            catch (EmailArgumentException emailArgumentException)
            {
                logger.LogError(emailArgumentException, "There was an exception.");
                LogEmailArgumentExceptionDebug(emailArgumentException);
                return NewEmailResult(emailArgumentException);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "There was an exception.");
                return NewEmailResult(ex);
            }
        }

        private void LogEmailArgumentExceptionDebug(EmailArgumentException emailArgumentException)
        {
            var builder = new StringBuilder();

            builder.AppendLine("There was a problem with the email settings.");
            builder.AppendLine("Ensure all of the email settings are configured and valid before trying again.");

            logger.LogTrace(emailArgumentException, builder.ToString());
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
            string fromAddress = ParseFromAddress(emailSettings.Address);
            string displayName = ParseFromDisplayName(emailSettings.DisplayName);

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

        private SmtpClient NewSmtpClient(MailAddress fromAddress)
        {
            string host = ParseSmtpHost(emailSettings.Host);
            int port = ParsePort(emailSettings.Port);
            string password = ParsePassword(emailSettings.Password);
            ICredentialsByHost credentials = NewNetworkCredential(fromAddress.Address, password);
            var deliveryMethod = SmtpDeliveryMethod.Network;
            var enableSsl = true;
            var useDefaultCredentials = false;

            logger.LogDebug(@"Preparing Smtp client:
    Host: {host}
    Port: {port}
    From Address: {address}
    Delivery Method: {deliveryMethod}
    Enable SSL: {enableSsl}
    Use Default Credential: {useDefaultCredentails}", host, port, fromAddress, enableSsl, useDefaultCredentials);

            logger.LogTrace("Prepared Smtp client with password {password}", password);

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

        private int ParsePort(int unsafePort)
        {
            return emailSettingsValidatorService.ParsePort(unsafePort);
        }

        private IEnumerable<string> ParseReceivers(ICollection<string> unsafeReceivers)
        {
            return emailSettingsValidatorService.ParseReceivers(unsafeReceivers);
        }

        private string ParseSmtpHost(string unsafeSmtpHost)
        {
            return emailSettingsValidatorService.ParseSmtpHost(unsafeSmtpHost);
        }

        private void SendMessage(SmtpClient smtpClient, MailAddress fromAddress, MailAddress toAddress, string subject,
                                 string body)
        {
            logger.LogDebug("Sending message to {address}", toAddress.Address);

            using (MailMessage mailMessage = NewMailMessage(fromAddress, toAddress, subject, body))
            {
                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (SmtpException smtpException)
                {
                    logger.LogError(smtpException, "An smtp exception was caught with a status code of {statusCode}",
                        smtpException.StatusCode);

                    SmtpStatusCode statusCode = smtpException.StatusCode;
                    if (statusCode == SmtpStatusCode.MailboxBusy || statusCode == SmtpStatusCode.MailboxUnavailable)
                    {
                        logger.LogWarning("Attempting to resend message to {address}", toAddress.Address);

                        Thread.Sleep(WaitTimeInMillisecondsBeforeTryingSendAgain);
                        smtpClient.Send(mailMessage);
                    }
                }
            }
        }
    }
}