namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Net;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public class SecureDownloadProvider : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ILoggingService loggingService;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadProvider(IDownloadService downloadService,
                                      ISecureFilePayloadService secureFilePayloadService,
                                      ILoggingService loggingService)
        {
            this.downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
            this.secureFilePayloadService = secureFilePayloadService
                                            ?? throw new ArgumentNullException(nameof(secureFilePayloadService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
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