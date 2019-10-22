namespace StatsDownload.Parsing
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FileValidationProvider : IFileValidationService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileReaderService fileReaderService;

        private readonly IStatsFileParserService statsFileParserService;

        public FileValidationProvider(IFileCompressionService fileCompressionService,
                                      IFileReaderService fileReaderService,
                                      IStatsFileParserService statsFileParserService)
        {
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
            this.statsFileParserService = statsFileParserService;
        }

        public ParseResults ValidateFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
            ParseResults parseResults = statsFileParserService.Parse(filePayload);
            filePayload.FileUtcDateTime = parseResults.DownloadDateTime;
            return parseResults;
        }
    }
}