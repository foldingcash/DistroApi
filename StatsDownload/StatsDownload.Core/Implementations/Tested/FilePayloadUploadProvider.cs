namespace StatsDownload.Core
{
    using System;

    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileReaderService fileReaderService;

        public FilePayloadUploadProvider(IFileCompressionService fileCompressionService,
                                         IFileReaderService fileReaderService,
                                         IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
            if (fileCompressionService == null)
            {
                throw new ArgumentNullException(nameof(fileCompressionService));
            }

            if (fileReaderService == null)
            {
                throw new ArgumentNullException(nameof(fileReaderService));
            }

            if (fileDownloadDataStoreService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadDataStoreService));
            }

            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
        }

        public void UploadFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload);
            fileReaderService.ReadFile(filePayload);
            fileDownloadDataStoreService.FileDownloadFinished(filePayload);
        }
    }
}