namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.Net;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Logging;

    public class SecureDownloadProvider : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ILoggingService loggingService;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadProvider(IDownloadService downloadService,
            ISecureFilePayloadService secureFilePayloadService, ILoggingService loggingService)
        {
            if (downloadService == null)
            {
                throw new ArgumentNullException(nameof(downloadService));
            }

            if (secureFilePayloadService == null)
            {
                throw new ArgumentNullException(nameof(secureFilePayloadService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
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
    }
}