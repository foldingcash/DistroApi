namespace StatsDownload.Core
{
    using System;

    using StatsDownload.Logging;

    public class ResourceCleanupProvider : IResourceCleanupService
    {
        private readonly IFileService fileService;

        private readonly ILoggingService loggingService;

        public ResourceCleanupProvider(IFileService fileService, ILoggingService loggingService)
        {
            if (fileService == null)
            {
                throw new ArgumentNullException(nameof(fileService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            this.fileService = fileService;
            this.loggingService = loggingService;
        }

        public void Cleanup(FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;

            string downloadFilePath = filePayload.DownloadFilePath;
            string decompressedDownloadFilePath = filePayload.DecompressedDownloadFilePath;
            string failedDownloadFilePath = filePayload.FailedDownloadFilePath;

            loggingService.LogVerbose($"{nameof(Cleanup)} Invoked");

            if (fileService.Exists(decompressedDownloadFilePath))
            {
                loggingService.LogVerbose($"Deleting: {decompressedDownloadFilePath}");
                fileService.Delete(decompressedDownloadFilePath);
            }

            if (fileService.Exists(downloadFilePath))
            {
                if (fileDownloadResult.FailedReason == FailedReason.FileDownloadFailedDecompression)
                {
                    loggingService.LogVerbose($"Moving: {downloadFilePath} to {failedDownloadFilePath}");
                    fileService.Move(downloadFilePath, failedDownloadFilePath);
                }
                else
                {
                    loggingService.LogVerbose($"Deleting: {downloadFilePath}");
                    fileService.Delete(downloadFilePath);
                }
            }
        }
    }
}