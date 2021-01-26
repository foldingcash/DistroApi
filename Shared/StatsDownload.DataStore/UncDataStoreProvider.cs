namespace StatsDownload.DataStore
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Settings;

    public class UncDataStoreProvider : IDataStoreService
    {
        private readonly IDirectoryService directoryService;

        private readonly IFileService fileService;

        private readonly ILogger logger;

        private readonly DataStoreSettings settings;

        public UncDataStoreProvider(ILogger<UncDataStoreProvider> logger, IOptions<DataStoreSettings> settings,
                                    IDirectoryService directoryService, IFileService fileService)
        {
            this.settings = settings.Value;
            this.directoryService = directoryService;
            this.fileService = fileService;
            this.logger = logger;
        }

        public Task DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            return Task.Run(() => fileService.CopyFile(validatedFile.FilePath, filePayload.UploadPath));
        }

        public Task<bool> IsAvailable()
        {
            string uploadDirectory = settings.UploadDirectory;
            bool directoryExists = directoryService.Exists(uploadDirectory);

            if (!directoryExists)
            {
                logger.LogWarning($"The path '{uploadDirectory}' does not exist");
            }

            return Task.FromResult(directoryExists);
        }

        public Task UploadFile(FilePayload filePayload)
        {
            return Task.Run(() => fileService.CopyFile(filePayload.DownloadFilePath, filePayload.UploadPath));
        }
    }
}