namespace StatsDownload.DataStore
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public class AzureDataStoreProvider : IDataStoreService
    {
        private ILoggingService logger;

        private IAzureDataStoreSettingsService settingsService;

        public AzureDataStoreProvider(ILoggingService logger, IAzureDataStoreSettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
        }

        public void DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }

        public void UploadFile(FilePayload filePayload)
        {
            throw new NotImplementedException();
        }
    }
}