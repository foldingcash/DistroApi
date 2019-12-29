namespace StatsDownload.DataStore
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    public class AzureDataStoreProvider : IDataStoreService
    {
        private readonly ILoggingService logger;

        private readonly IAzureDataStoreSettingsService settingsService;

        public AzureDataStoreProvider(ILoggingService logger, IAzureDataStoreSettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
        }

        public async Task DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            await GetContainerAndExecute(async container =>
            {
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(validatedFile.FilePath);
                await cloudBlockBlob.DownloadToFileAsync(filePayload.DownloadFilePath, FileMode.CreateNew);
                return true;
            });
        }

        public async Task<bool> IsAvailable()
        {
            return await GetContainerAndExecute(async container =>
            {
                bool isAvailable = await container.ExistsAsync();

                if (!isAvailable)
                {
                    isAvailable = await container.CreateIfNotExistsAsync();
                }

                return isAvailable;
            });
        }

        public async Task UploadFile(FilePayload filePayload)
        {
            await GetContainerAndExecute(async container =>
            {
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filePayload.UploadPath);
                await cloudBlockBlob.UploadFromFileAsync(filePayload.DownloadFilePath);
                return true;
            });
        }

        private async Task<T> GetContainerAndExecute<T>(Func<CloudBlobContainer, Task<T>> func)
        {
            string storageConnectionString = settingsService.ConnectionString;
            CloudStorageAccount storageAccount;

            try
            {
                logger.LogVerbose("Attempting to parse storage connection string");
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (Exception)
            {
                logger.LogVerbose($"Failed to parse storage connection string: {storageConnectionString}");
                throw;
            }

            CloudBlobContainer cloudBlobContainer;

            try
            {
                logger.LogVerbose($"Attempting to get container reference: {settingsService.ContainerName}");
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                cloudBlobContainer = cloudBlobClient.GetContainerReference(settingsService.ContainerName);
                logger.LogVerbose("Successfully retrieved container reference");
            }
            catch (Exception)
            {
                logger.LogVerbose($"Failed to get container reference: {settingsService.ContainerName}");
                throw;
            }

            try
            {
                return await func(cloudBlobContainer);
            }
            catch (Exception)
            {
                logger.LogVerbose("Failed to execute func");
                throw;
            }
        }
    }
}