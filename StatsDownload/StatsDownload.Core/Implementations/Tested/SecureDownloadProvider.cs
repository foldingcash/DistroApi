namespace StatsDownload.Core
{
    using System;
    using System.Net;

    public class SecureDownloadProvider : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadProvider(
            IDownloadService downloadService,
            ISecureFilePayloadService secureFilePayloadService)
        {
            if (IsNull(downloadService))
            {
                throw NewArgumentNullException(nameof(downloadService));
            }

            if (IsNull(secureFilePayloadService))
            {
                throw NewArgumentNullException(nameof(secureFilePayloadService));
            }

            this.downloadService = downloadService;
            this.secureFilePayloadService = secureFilePayloadService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            bool originalWasNotSecure = !secureFilePayloadService.IsSecureConnection(filePayload);

            try
            {
                secureFilePayloadService.EnableSecureFilePayload(filePayload);
                downloadService.DownloadFile(filePayload);
            }
            catch (WebException)
            {
                if (originalWasNotSecure)
                {
                    secureFilePayloadService.DisableSecureFilePayload(filePayload);
                    downloadService.DownloadFile(filePayload);
                }
            }
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }
    }
}