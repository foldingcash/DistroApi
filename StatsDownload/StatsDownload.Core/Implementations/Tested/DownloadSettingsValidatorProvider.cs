namespace StatsDownload.Core
{
    public class DownloadSettingsValidatorProvider : IDownloadSettingsValidatorService
    {
        private const int MaximumTimeout = 3600;

        private const int MinimumTimeout = 100;

        public bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return bool.TryParse(unsafeAcceptAnySslCert, out acceptAnySslCert);
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