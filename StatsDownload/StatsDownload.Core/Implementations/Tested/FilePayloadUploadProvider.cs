namespace StatsDownload.Core
{
    public class FilePayloadUploadProvider : IFilePayloadUploadService
    {
        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileReaderService fileReaderService;

        public FilePayloadUploadProvider(
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService,
            IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
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