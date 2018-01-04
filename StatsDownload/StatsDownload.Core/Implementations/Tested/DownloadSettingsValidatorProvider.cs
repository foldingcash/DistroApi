namespace StatsDownload.Core
{
    using System;

    public class DownloadSettingsValidatorProvider : IDownloadSettingsValidatorService
    {
        private const int MaximumTimeout = 3600;

        private const int MaximumWaitTimeInHours = 100;

        private const int MinimumTimeout = 100;

        private const int MinimumWaitTimeInHours = 1;

        public bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return bool.TryParse(unsafeAcceptAnySslCert, out acceptAnySslCert);
        }

        public bool TryParseMinimumWaitTimeSpan(string unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan)
        {
            int minimumWaitTimeInHours;
            if (int.TryParse(unsafeMinimumWaitTimeInHours, out minimumWaitTimeInHours))
            {
                if (minimumWaitTimeInHours >= MinimumWaitTimeInHours && minimumWaitTimeInHours <= MaximumWaitTimeInHours)
                {
                    minimumWaitTimeSpan = new TimeSpan(minimumWaitTimeInHours, 0, 0);
                    return true;
                }
            }

            minimumWaitTimeSpan = TimeSpan.Zero;
            return false;
        }

        public bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            bool parsed = int.TryParse(unsafeTimeout, out timeoutInSeconds);

            if (!parsed)
            {
                return false;
            }

            if (timeoutInSeconds < MinimumTimeout || timeoutInSeconds > MaximumTimeout)
            {
                return false;
            }

            return true;
        }
    }
}