namespace StatsDownload.Core.Implementations.Untested
{
    using System.IO;
    using Interfaces.DataTransfer;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Logging;

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
            loggingService.LogVerbose($"Attempting to read file contents: {dateTimeService.DateTimeNow()}");

            using (var reader = new StreamReader(filePayload.DecompressedDownloadFilePath))
            {
                filePayload.DecompressedDownloadFileData = reader.ReadToEnd();
            }

            loggingService.LogVerbose($"Reading file complete: {dateTimeService.DateTimeNow()}");
        }
    }
}