namespace StatsDownload.Core
{
    using System;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
        }

        public bool IsMinimumWaitTimeMet()
        {
            throw new NotImplementedException();
        }
    }
}