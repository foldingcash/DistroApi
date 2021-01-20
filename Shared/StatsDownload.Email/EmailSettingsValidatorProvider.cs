namespace StatsDownload.Email
{
    using System.Collections.Generic;
    using System.Linq;

    public class EmailSettingsValidatorProvider : IEmailSettingsValidatorService
    {
        public string ParseFromAddress(string unsafeFromAddress)
        {
            if (string.IsNullOrWhiteSpace(unsafeFromAddress))
            {
                throw new EmailArgumentException("A from email address was not provided");
            }

            return unsafeFromAddress;
        }

        public string ParseFromDisplayName(string unsafeFromDisplayName)
        {
            if (string.IsNullOrWhiteSpace(unsafeFromDisplayName))
            {
                throw new EmailArgumentException("A from display name was not provided");
            }

            return unsafeFromDisplayName;
        }

        public string ParsePassword(string unsafePassword)
        {
            if (string.IsNullOrWhiteSpace(unsafePassword))
            {
                throw new EmailArgumentException("A password was not provided");
            }

            return unsafePassword;
        }

        public int ParsePort(int unsafePort)
        {
            if (unsafePort < 1 || unsafePort > 65535)
            {
                throw new EmailArgumentException("The port should be between 1 and 65535, inclusive");
            }

            return unsafePort;
        }

        public IEnumerable<string> ParseReceivers(ICollection<string> unsafeReceivers)
        {
            IEnumerable<string> filtered = unsafeReceivers?.Where(receiver => !string.IsNullOrEmpty(receiver));

            if (!filtered?.Any() ?? true)
            {
                throw new EmailArgumentException("A receivers list was not provided");
            }

            return filtered;
        }

        public string ParseSmtpHost(string unsafeSmtpHost)
        {
            if (string.IsNullOrWhiteSpace(unsafeSmtpHost))
            {
                throw new EmailArgumentException("A SMTP host was not provided");
            }

            return unsafeSmtpHost;
        }
    }
}