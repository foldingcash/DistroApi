namespace StatsDownload.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;

    public class EmailProvider : IEmailService
    {
        private readonly IEmailCredentials credentials;

        public EmailProvider(IEmailCredentials credentials)
        {
            this.credentials = credentials;
        }

        public EmailResult SendEmail(string name, string email, string subject, string body)
        {
            try
            {
                MailAddress fromAddress = NewMailAddress(credentials.Address, credentials.DisplayName);
                MailAddress toAddress = NewMailAddress(email, name);

                SendMessage(fromAddress, toAddress, subject, body);

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

        private MailMessage NewMailMessage(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            return new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body };
        }

        private ICredentialsByHost NewNetworkCredential(string userName, string password)
        {
            return new NetworkCredential(userName, password);
        }

        private void SendMessage(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            using (
                var smtpClient = new SmtpClient
                                     {
                                         Host = credentials.HostName, Port = credentials.Port, EnableSsl = true,
                                         DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false,
                                         Credentials = NewNetworkCredential(fromAddress.Address, credentials.Password)
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