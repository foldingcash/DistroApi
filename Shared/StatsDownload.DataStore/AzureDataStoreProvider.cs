namespace StatsDownload.DataStore
{
    using System;
    using System.IO;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public class AzureDataStoreProvider : IDataStoreService
    {
        private readonly IAzureDataStoreSettingsService settingsService;

        private ILoggingService logger;

        public AzureDataStoreProvider(ILoggingService logger, IAzureDataStoreSettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
        }

        public void DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            GetContainerAndExecute(async container =>
            {
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(validatedFile.FilePath);

                await cloudBlockBlob.DownloadToFileAsync(filePayload.DownloadFilePath, FileMode.CreateNew);
            });
        }

        public bool IsAvailable()
        {
            var isAvailable = false;
            GetContainerAndExecute(async container => { isAvailable = await container.CreateIfNotExistsAsync(); });

            return isAvailable;
        }

        public void UploadFile(FilePayload filePayload)
        {
            GetContainerAndExecute(async container =>
            {
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filePayload.UploadPath);
                await cloudBlockBlob.UploadFromFileAsync(filePayload.DownloadFilePath);
            });
        }

        private void GetContainerAndExecute(Action<CloudBlobContainer> action)
        {
            string storageConnectionString = settingsService.ConnectionString;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer cloudBlobContainer =
                cloudBlobClient.GetContainerReference(settingsService.ContainerName);

            action(cloudBlobContainer);
        }
    }
}