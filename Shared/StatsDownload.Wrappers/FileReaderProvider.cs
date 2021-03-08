namespace StatsDownload.Wrappers
{
    using System.IO;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Logging;

    public class FileReaderProvider : IFileReaderService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILogger<FileReaderProvider> logger;

        public FileReaderProvider(ILogger<FileReaderProvider> logger, IDateTimeService dateTimeService)
        {
            this.logger = logger;
            this.dateTimeService = dateTimeService;
        }

        public void ReadFile(FilePayload filePayload)
        {
            logger.LogMethodInvoked();
            ReadDecompressedFile(filePayload);
            logger.LogMethodFinished();
        }

        private void ReadDecompressedFile(FilePayload filePayload)
        {
            logger.LogDebug($"Attempting to read file contents: {dateTimeService.DateTimeNow()}");

            filePayload.DecompressedDownloadFileData = File.ReadAllText(filePayload.DecompressedDownloadFilePath);

            logger.LogDebug($"Reading file complete: {dateTimeService.DateTimeNow()}");
        }
    }
}