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
            if (IsNull(fileService))
            {
                throw NewArgumentNullException(nameof(fileService));
            }

            if (IsNull(loggingService))
            {
                throw NewArgumentNullException(nameof(loggingService));
            }

            this.fileService = fileService;
            this.loggingService = loggingService;
        }

        public void Cleanup(FilePayload filePayload)
        {
            string downloadFilePath = filePayload.DownloadFilePath;
            string decompressedDownloadFilePath = filePayload.DecompressedDownloadFilePath;

            loggingService.LogVerbose($"{nameof(Cleanup)} Invoked");

            if (fileService.Exists(decompressedDownloadFilePath))
            {
                loggingService.LogVerbose($"Deleting: {decompressedDownloadFilePath}");
                fileService.Delete(decompressedDownloadFilePath);
            }

            if (fileService.Exists(downloadFilePath))
            {
                loggingService.LogVerbose($"Deleting: {downloadFilePath}");
                fileService.Delete(downloadFilePath);
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