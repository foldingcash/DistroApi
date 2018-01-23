namespace StatsDownload.Email
{
    using System;
    using System.Collections.Generic;

    public class EmailSettingsValidatorProvider : IEmailSettingsValidatorService
    {
        public int ParsePort(string unsafePort)
        {
            int port;
            if (!int.TryParse(unsafePort, out port))
            {
                throw new ArgumentException("An integer was not provided", nameof(unsafePort));
            }

            if (port < 1 || port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(unsafePort),
                    "The port should be between 1 and 65535, inclusive");
            }

            return port;
        }

        public IEnumerable<string> ParseReceivers(string unsafeReceivers)
        {
            return unsafeReceivers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}