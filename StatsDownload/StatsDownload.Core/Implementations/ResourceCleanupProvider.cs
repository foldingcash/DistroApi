namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;

    public class ResourceCleanupProvider : IResourceCleanupService
    {
        private readonly IFileService fileService;

        private readonly ILoggingService loggingService;

        public ResourceCleanupProvider(IFileService fileService, ILoggingService loggingService)
        {
            this.fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }
        
        public void Cleanup(FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;

            string downloadFilePath = filePayload.DownloadFilePath;
            string decompressedDownloadFilePath = filePayload.DecompressedDownloadFilePath;
            string failedDownloadFilePath = filePayload.FailedDownloadFilePath;

            LogVerbose($"{nameof(Cleanup)} Invoked");

            if (Exists(decompressedDownloadFilePath))
            {
                LogVerbose($"Deleting: {decompressedDownloadFilePath}");
                Delete(decompressedDownloadFilePath);
            }

            if (Exists(downloadFilePath))
            {
                if (fileDownloadResult.FailedReason == FailedReason.FileDownloadFailedDecompression)
                {
                    LogVerbose($"Moving: {downloadFilePath} to {failedDownloadFilePath}");
                    Move(downloadFilePath, failedDownloadFilePath);
                }
                else
                {
                    LogVerbose($"Deleting: {downloadFilePath}");
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

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private void Move(string sourcePath, string destinationPath)
        {
            fileService.Move(sourcePath, destinationPath);
        }
    }
}