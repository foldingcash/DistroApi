namespace StatsDownload.Parsing
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class DownloadSettingsValidatorProvider : IDownloadSettingsValidatorService
    {
        private readonly IDirectoryService directoryService;

        public DownloadSettingsValidatorProvider(IDirectoryService directoryService)
        {
            this.directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
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
                if (minimumWaitTimeInHours >= Constants.Download.MinimumWaitTimeInHours
                    && minimumWaitTimeInHours <= Constants.Download.MaximumWaitTimeInHours)
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

            if (timeoutInSeconds < Constants.Download.MinimumTimeout
                || timeoutInSeconds > Constants.Download.MaximumTimeout)
            {
                return false;
            }

            return true;
        }
    }
}