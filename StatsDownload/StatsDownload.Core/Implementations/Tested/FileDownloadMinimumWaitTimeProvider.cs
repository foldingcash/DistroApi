namespace StatsDownload.Core
{
    using System;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly TimeSpan MinimumWaitTimeSpan = new TimeSpan(1, 0, 0);

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            TimeSpan configuredMinimumWaitTimeSpan = filePayload.MinimumWaitTimeSpan;
            DateTime lastSuccessfulRun = fileDownloadDataStoreService.GetLastFileDownloadDateTime();
            DateTime dateTimeNow = DateTime.Now;

            if (dateTimeNow - lastSuccessfulRun < MinimumWaitTimeSpan)
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