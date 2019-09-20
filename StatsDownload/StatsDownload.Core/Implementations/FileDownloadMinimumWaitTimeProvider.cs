namespace StatsDownload.Core.Implementations
{
    using System;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class FileDownloadMinimumWaitTimeProvider : IFileDownloadMinimumWaitTimeService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        public FileDownloadMinimumWaitTimeProvider(IFileDownloadDatabaseService fileDownloadDatabaseService,
                                                   IDateTimeService dateTimeService)
        {
            this.fileDownloadDatabaseService = fileDownloadDatabaseService ?? throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
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