namespace StatsDownload.DataStore
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class UncDataStoreProvider : IDataStoreService
    {
        private readonly IDirectoryService directoryService;

        private readonly IFileService fileService;

        private readonly DataStoreSettings settings;

        public UncDataStoreProvider(IOptions<DataStoreSettings> settings, IDirectoryService directoryService,
                                    IFileService fileService)
        {
            this.settings = settings.Value;
            this.directoryService = directoryService;
            this.fileService = fileService;
        }

        public Task DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            return Task.Run(() => fileService.CopyFile(validatedFile.FilePath, filePayload.UploadPath));
        }

        public Task<bool> IsAvailable()
        {
            string uploadDirectory = settings.UploadDirectory;
            return Task.FromResult(directoryService.Exists(uploadDirectory));
        }

        public Task UploadFile(FilePayload filePayload)
        {
            return Task.Run(() => fileService.CopyFile(filePayload.DownloadFilePath, filePayload.UploadPath));
        }
    }
}