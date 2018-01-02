namespace StatsDownload.Core
{
    using System;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly TimeSpan MinimumWaitTime = new TimeSpan(1, 0, 0);

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDataStoreService fileDownloadDataStoreService)
        {
            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            TimeSpan configuredWaitTime = filePayload.MinimumWaitTime;
            DateTime lastSuccessfulRun = fileDownloadDataStoreService.GetLastFileDownloadDateTime();
            DateTime dateTimeNow = DateTime.Now;

            if (dateTimeNow - lastSuccessfulRun < MinimumWaitTime)
            {
                return false;
            }

            if (dateTimeNow - lastSuccessfulRun < configuredWaitTime)
            {
                return false;
            }

            return true;
        }
    }
}