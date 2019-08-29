namespace StatsDownload.Core.Implementations
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FileValidationProvider : IFileValidationService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileReaderService fileReaderService;

        public FileValidationProvider(IFileCompressionService fileCompressionService,
                                      IFileReaderService fileReaderService)
        {
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
        }

        public void ValidateFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
        }
    }
}