namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IDataStoreService dataStoreService;

        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        private readonly IFileReaderService fileReaderService;

        public FilePayloadUploadProvider(IFileCompressionService fileCompressionService,
                                         IFileReaderService fileReaderService,
                                         IFileDownloadDatabaseService fileDownloadDatabaseService,
                                         IDataStoreService dataStoreService)
        {
            this.fileCompressionService =
                fileCompressionService ?? throw new ArgumentNullException(nameof(fileCompressionService));
            this.fileReaderService = fileReaderService ?? throw new ArgumentNullException(nameof(fileReaderService));
            this.fileDownloadDatabaseService = fileDownloadDatabaseService
                                               ?? throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            this.dataStoreService = dataStoreService ?? throw new ArgumentNullException(nameof(dataStoreService));
        }

        public void UploadFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload.DownloadFilePath,
                filePayload.DecompressedDownloadFilePath);
            fileReaderService.ReadFile(filePayload);
            dataStoreService.UploadFile(filePayload);
            fileDownloadDatabaseService.FileDownloadFinished(filePayload);
        }
    }
}