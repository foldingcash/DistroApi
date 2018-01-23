namespace StatsDownload.Core
{
    using System;
    using System.Net;

    using StatsDownload.Logging;

    public class SecureDownloadProvider : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ILoggingService loggingService;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadProvider(IDownloadService downloadService,
                                      ISecureFilePayloadService secureFilePayloadService, ILoggingService loggingService)
        {
            if (IsNull(downloadService))
            {
                throw NewArgumentNullException(nameof(downloadService));
            }

            if (IsNull(secureFilePayloadService))
            {
                throw NewArgumentNullException(nameof(secureFilePayloadService));
            }

            if (IsNull(loggingService))
            {
                throw NewArgumentNullException(nameof(loggingService));
            }

            this.downloadService = downloadService;
            this.secureFilePayloadService = secureFilePayloadService;
            this.loggingService = loggingService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            bool originalWasNotSecure = !secureFilePayloadService.IsSecureConnection(filePayload);

            try
            {
                secureFilePayloadService.EnableSecureFilePayload(filePayload);
                downloadService.DownloadFile(filePayload);
            }
            catch (WebException webException)
            {
                loggingService.LogException(webException);

                if (originalWasNotSecure)
                {
                    secureFilePayloadService.DisableSecureFilePayload(filePayload);
                    downloadService.DownloadFile(filePayload);
                }
                else
                {
                    throw;
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