namespace StatsDownload.Wrappers
{
    using System.IO;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public class FileReaderProvider : IFileReaderService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly ILoggingService loggingService;

        public FileReaderProvider(ILoggingService loggingService, IDateTimeService dateTimeService)
        {
            this.loggingService = loggingService;
            this.dateTimeService = dateTimeService;
        }

        public void ReadFile(FilePayload filePayload)
        {
            ReadDecompressedFile(filePayload);
            ReadCompressedFile(filePayload);
        }

        private void ReadCompressedFile(FilePayload filePayload)
        {
            loggingService.LogDebug($"Attempting to read compressed file contents: {dateTimeService.DateTimeNow()}");

            filePayload.DownloadFileData = File.ReadAllBytes(filePayload.DownloadFilePath);

            loggingService.LogDebug($"Reading file complete: {dateTimeService.DateTimeNow()}");
        }

        private void ReadDecompressedFile(FilePayload filePayload)
        {
            loggingService.LogDebug($"Attempting to read file contents: {dateTimeService.DateTimeNow()}");

            filePayload.DecompressedDownloadFileData = File.ReadAllText(filePayload.DecompressedDownloadFilePath);

            loggingService.LogDebug($"Reading file complete: {dateTimeService.DateTimeNow()}");
        }
    }
}