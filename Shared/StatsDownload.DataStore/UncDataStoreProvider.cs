namespace StatsDownload.DataStore
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class UncDataStoreProvider : IDataStoreService
    {
        private readonly IDirectoryService directoryService;

        private readonly IFileService fileService;

        private readonly IDataStoreSettings settings;

        public UncDataStoreProvider(IDataStoreSettings settings, IDirectoryService directoryService,
                                    IFileService fileService)
        {
            this.settings = settings;
            this.directoryService = directoryService;
            this.fileService = fileService;
        }

        public void DownloadFile(FilePayload filePayload)
        {
            fileService.CopyFile(filePayload.UploadPath, filePayload.DownloadFilePath);
        }

        public bool IsAvailable()
        {
            string uploadDirectory = settings.UploadDirectory;
            return directoryService.Exists(uploadDirectory);
        }

        public void UploadFile(FilePayload filePayload)
        {
            fileService.CopyFile(filePayload.DownloadFilePath, filePayload.UploadPath);
        }
    }
}