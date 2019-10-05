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
        private readonly IAzureDataStoreSettingsService settingsService;

        private ILoggingService logger;

        public AzureDataStoreProvider(ILoggingService logger, IAzureDataStoreSettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
        }

        public async void DownloadFile(FilePayload filePayload, ValidatedFile validatedFile)
        {
            string storageConnectionString = settingsService.ConnectionString;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob storage here.

                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(settingsService.ContainerName);

                // Get a reference to the blob address, then upload the file to the blob.
                // Use the value of localFileName for the blob name.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(validatedFile.FilePath);

                await cloudBlockBlob.DownloadToFileAsync(filePayload.DownloadFilePath, FileMode.CreateNew);
            }

            // Otherwise, let the user know that they need to define the environment variable.
            Console.WriteLine("A connection string has not been defined in the system environment variables. "
                              + "Add an environment variable named 'storageconnectionstring' with your storage "
                              + "connection string as a value.");
            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
        }

        public bool IsAvailable()
        {
            Task<bool> task = IsAvailableAsync();
            Task.WaitAll(task);
            return task.Result;
        }

        public async void UploadFile(FilePayload filePayload)
        {
            string storageConnectionString = settingsService.ConnectionString;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob storage here.

                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(settingsService.ContainerName);

                // Get a reference to the blob address, then upload the file to the blob.
                // Use the value of localFileName for the blob name.
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filePayload.UploadPath);
                await cloudBlockBlob.UploadFromFileAsync(filePayload.DownloadFilePath);
            }

            // Otherwise, let the user know that they need to define the environment variable.
            Console.WriteLine("A connection string has not been defined in the system environment variables. "
                              + "Add an environment variable named 'storageconnectionstring' with your storage "
                              + "connection string as a value.");
            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
        }

        private async Task<bool> IsAvailableAsync()
        {
            string storageConnectionString = settingsService.ConnectionString;

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob storage here.

                // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                CloudBlobContainer cloudBlobContainer =
                    cloudBlobClient.GetContainerReference(settingsService.ContainerName);
                return await cloudBlobContainer.ExistsAsync();
            }

            // Otherwise, let the user know that they need to define the environment variable.
            Console.WriteLine("A connection string has not been defined in the system environment variables. "
                              + "Add an environment variable named 'storageconnectionstring' with your storage "
                              + "connection string as a value.");
            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
            return false;
        }
    }
}