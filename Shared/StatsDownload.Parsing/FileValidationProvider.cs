namespace StatsDownload.Parsing
{
    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Logging;

    public class FileValidationProvider : IFileValidationService
    {
        private readonly ILogger<FileValidationProvider> logger;

        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileReaderService fileReaderService;

        private readonly IStatsFileParserService statsFileParserService;

        public FileValidationProvider(ILogger<FileValidationProvider> logger, IFileCompressionService fileCompressionService,
                                      IFileReaderService fileReaderService,
                                      IStatsFileParserService statsFileParserService)
        {
            this.logger = logger;
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
            this.statsFileParserService = statsFileParserService;
        }

        public ParseResults ValidateFile(FilePayload filePayload)
        {
            logger.LogMethodInvoked();
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
            ParseResults parseResults = statsFileParserService.Parse(filePayload);
            filePayload.FileUtcDateTime = parseResults.DownloadDateTime;
            logger.LogMethodFinished();
            return parseResults;
        }
    }
}