namespace StatsDownload.Core
{
    using System;

    using StatsDownload.Logging;

    public class ResourceCleanupProvider : IResourceCleanupService
    {
        private readonly IFileDeleteService fileDeleteService;

        private readonly ILoggingService loggingService;

        public ResourceCleanupProvider(IFileDeleteService fileDeleteService, ILoggingService loggingService)
        {
            if (IsNull(fileDeleteService))
            {
                throw NewArgumentNullException(nameof(fileDeleteService));
            }

            if (IsNull(loggingService))
            {
                throw NewArgumentNullException(nameof(loggingService));
            }

            this.fileDeleteService = fileDeleteService;
            this.loggingService = loggingService;
        }

        public void Cleanup(FilePayload filePayload)
        {
            string downloadFilePath = filePayload.DownloadFilePath;
            string decompressedDownloadFilePath = filePayload.DecompressedDownloadFilePath;

            loggingService.LogVerbose($"{nameof(Cleanup)} Invoked");

            if (fileDeleteService.Exists(decompressedDownloadFilePath))
            {
                loggingService.LogVerbose($"Deleting: {decompressedDownloadFilePath}");
                fileDeleteService.Delete(decompressedDownloadFilePath);
            }

            if (fileDeleteService.Exists(downloadFilePath))
            {
                loggingService.LogVerbose($"Deleting: {downloadFilePath}");
                fileDeleteService.Delete(downloadFilePath);
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