namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Net;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class SecureDownloadProvider : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ILogger logger;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadProvider(ILogger<SecureDownloadProvider> logger, IDownloadService downloadService,
                                      ISecureFilePayloadService secureFilePayloadService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
            this.secureFilePayloadService = secureFilePayloadService
                                            ?? throw new ArgumentNullException(nameof(secureFilePayloadService));
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
                logger.LogError(webException, "There was a web exception while trying to securely download the file");

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