namespace StatsDownload.Email
{
    using System;
    using System.Collections.Generic;

    public class EmailSettingsValidatorProvider : IEmailSettingsValidatorService
    {
        public string ParseFromAddress(string unsafeFromAddress)
        {
            if (string.IsNullOrWhiteSpace(unsafeFromAddress))
            {
                throw new EmailArgumentException(nameof(unsafeFromAddress), "A from email address was not provided");
            }

            return unsafeFromAddress;
        }

        public string ParseFromDisplayName(string unsafeFromDisplayName)
        {
            if (string.IsNullOrWhiteSpace(unsafeFromDisplayName))
            {
                throw new EmailArgumentException(nameof(unsafeFromDisplayName), "A from display name was not provided");
            }

            return unsafeFromDisplayName;
        }

        public string ParsePassword(string unsafePassword)
        {
            if (string.IsNullOrWhiteSpace(unsafePassword))
            {
                throw new EmailArgumentException(nameof(unsafePassword), "A password was not provided");
            }

            return unsafePassword;
        }

        public int ParsePort(string unsafePort)
        {
            int port;
            if (!int.TryParse(unsafePort, out port))
            {
                throw new EmailArgumentException(nameof(unsafePort), "An integer was not provided");
            }

            if (port < 1 || port > 65535)
            {
                throw new EmailArgumentException(nameof(unsafePort), "The port should be between 1 and 65535, inclusive");
            }

            return port;
        }

        public IEnumerable<string> ParseReceivers(string unsafeReceivers)
        {
            if (string.IsNullOrWhiteSpace(unsafeReceivers))
            {
                throw new EmailArgumentException(nameof(unsafeReceivers), "A receivers list was not provided");
            }

            return unsafeReceivers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string ParseSmtpHost(string unsafeSmtpHost)
        {
            if (string.IsNullOrWhiteSpace(unsafeSmtpHost))
            {
                throw new EmailArgumentException(nameof(unsafeSmtpHost), "A SMTP host was not provided");
            }

            return unsafeSmtpHost;
        }
    }
}