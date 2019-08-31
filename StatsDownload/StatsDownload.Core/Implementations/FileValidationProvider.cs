namespace StatsDownload.Core.Implementations
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

        public void ValidateFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
            filePayload.FileUtcDateTime = statsFileParserService.Parse(filePayload).DownloadDateTime;
        }
    }
}