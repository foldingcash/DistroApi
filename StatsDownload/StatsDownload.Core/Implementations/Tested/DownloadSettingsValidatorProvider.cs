namespace StatsDownload.Core.Implementations.Tested
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class DownloadSettingsValidatorProvider : IDownloadSettingsValidatorService
    {
        private const int MaximumTimeout = 3600;

        private const int MaximumWaitTimeInHours = 100;

        private const int MinimumTimeout = 100;

        private const int MinimumWaitTimeInHours = 1;

        private readonly IDirectoryService directoryService;

        public DownloadSettingsValidatorProvider(IDirectoryService directoryService)
        {
            if (directoryService == null)
            {
                throw new ArgumentNullException(nameof(directoryService));
            }

            this.directoryService = directoryService;
        }

        public bool IsValidDownloadDirectory(string unsafeDownloadDirectory)
        {
            return directoryService.Exists(unsafeDownloadDirectory);
        }

        public bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return bool.TryParse(unsafeAcceptAnySslCert, out acceptAnySslCert);
        }

        public bool TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri)
        {
            return Uri.TryCreate(unsafeDownloadUri, UriKind.RelativeOrAbsolute, out downloadUri);
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