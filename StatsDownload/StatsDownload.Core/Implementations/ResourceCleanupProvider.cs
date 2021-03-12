namespace StatsDownload.Core.Implementations
{
    using System;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public class ResourceCleanupProvider : IResourceCleanupService
    {
        private readonly IFileService fileService;

        private readonly ILogger logger;

        public ResourceCleanupProvider(ILogger<ResourceCleanupProvider> logger, IFileService fileService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public void Cleanup(FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;

            string downloadFilePath = filePayload.DownloadFilePath;
            string decompressedDownloadFilePath = filePayload.DecompressedDownloadFilePath;
            string failedDownloadFilePath = filePayload.FailedDownloadFilePath;

            LogDebug($"{nameof(Cleanup)} Invoked");

            if (Exists(decompressedDownloadFilePath))
            {
                LogDebug($"Deleting: {decompressedDownloadFilePath}");
                Delete(decompressedDownloadFilePath);
            }

            if (Exists(downloadFilePath))
            {
                if (fileDownloadResult.FailedReason == FailedReason.FileDownloadFailedDecompression)
                {
                    LogDebug($"Moving: {downloadFilePath} to {failedDownloadFilePath}");
                    Move(downloadFilePath, failedDownloadFilePath);
                }
                else
                {
                    LogDebug($"Deleting: {downloadFilePath}");
                    Delete(downloadFilePath);
                }
            }
        }

        private void Delete(string path)
        {
            fileService.Delete(path);
        }

        private bool Exists(string path)
        {
            return fileService.Exists(path);
        }

        private void LogDebug(string message)
        {
            logger.LogDebug(message);
        }

        private void Move(string sourcePath, string destinationPath)
        {
            fileService.Move(sourcePath, destinationPath);
        }
    }
}