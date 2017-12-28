namespace StatsDownload.Core
{
    using System.Net;

    public class SecureDownloadService : IDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly ISecureDetectionService secureDetectionService;

        private readonly ISecureFilePayloadService secureFilePayloadService;

        public SecureDownloadService(
            IDownloadService downloadService,
            ISecureDetectionService secureDetectionService,
            ISecureFilePayloadService secureFilePayloadService)
        {
            this.downloadService = downloadService;
            this.secureDetectionService = secureDetectionService;
            this.secureFilePayloadService = secureFilePayloadService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            bool isOriginalSecure = secureDetectionService.IsSecureConnection(filePayload);

            try
            {
                secureFilePayloadService.EnableSecureFilePayload(filePayload);
                downloadService.DownloadFile(filePayload);
            }
            catch (WebException)
            {
                if (!isOriginalSecure)
                {
                    secureFilePayloadService.DisableSecureFilePayload(filePayload);
                    downloadService.DownloadFile(filePayload);
                }
            }
        }
    }
}