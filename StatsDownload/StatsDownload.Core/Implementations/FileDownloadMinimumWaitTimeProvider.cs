namespace StatsDownload.Core.Implementations
{
    using System;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDatabaseService fileDownloadDatabaseService,
            IDateTimeService dateTimeService)
        {
            if (fileDownloadDatabaseService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            }

            if (dateTimeService == null)
            {
                throw new ArgumentNullException(nameof(dateTimeService));
            }

            this.fileDownloadDatabaseService = fileDownloadDatabaseService;
            this.dateTimeService = dateTimeService;
        }

        public bool IsMinimumWaitTimeMet(FilePayload filePayload)
        {
            TimeSpan configuredMinimumWaitTimeSpan = filePayload.MinimumWaitTimeSpan;
            DateTime lastSuccessfulRun = fileDownloadDatabaseService.GetLastFileDownloadDateTime();
            DateTime dateTimeNow = dateTimeService.DateTimeNow();

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