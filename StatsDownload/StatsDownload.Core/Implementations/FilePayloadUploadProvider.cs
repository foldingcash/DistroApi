namespace StatsDownload.Core.Implementations
{
    using System;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        private readonly IFileReaderService fileReaderService;

        public FilePayloadUploadProvider(IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService,
            IFileDownloadDatabaseService fileDownloadDatabaseService)
        {
            this.fileCompressionService = fileCompressionService ??
                                          throw new ArgumentNullException(nameof(fileCompressionService));
            this.fileReaderService = fileReaderService ?? throw new ArgumentNullException(nameof(fileReaderService));
            this.fileDownloadDatabaseService = fileDownloadDatabaseService ??
                                               throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
        }

        public void UploadFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
            fileDownloadDatabaseService.FileDownloadFinished(filePayload);
        }
    }
}