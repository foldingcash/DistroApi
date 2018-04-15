namespace StatsDownload.Core
{
    using System;

    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        private readonly IFileReaderService fileReaderService;

        public FilePayloadUploadProvider(IFileCompressionService fileCompressionService,
                                         IFileReaderService fileReaderService,
                                         IFileDownloadDatabaseService fileDownloadDatabaseService)
        {
            if (fileCompressionService == null)
            {
                throw new ArgumentNullException(nameof(fileCompressionService));
            }

            if (fileReaderService == null)
            {
                throw new ArgumentNullException(nameof(fileReaderService));
            }

            if (fileDownloadDatabaseService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            }

            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
            this.fileDownloadDatabaseService = fileDownloadDatabaseService;
        }

        public void UploadFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload);
            fileReaderService.ReadFile(filePayload);
            fileDownloadDatabaseService.FileDownloadFinished(filePayload);
        }
    }
}