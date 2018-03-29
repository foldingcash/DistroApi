namespace StatsDownload.Core
{
    using System;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
            if (fileDownloadDataStoreService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadDataStoreService));
            }

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            TimeSpan configuredMinimumWaitTimeSpan = filePayload.MinimumWaitTimeSpan;
            DateTime lastSuccessfulRun = fileDownloadDataStoreService.GetLastFileDownloadDateTime();
            DateTime dateTimeNow = DateTime.Now;

            if (dateTimeNow - lastSuccessfulRun < MinimumWait.TimeSpan)
            {
                return false;
            }

            if (dateTimeNow - lastSuccessfulRun < configuredMinimumWaitTimeSpan)
            {
                return false;
            }

            return true;
        }
    }
}