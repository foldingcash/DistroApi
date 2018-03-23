namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;

    public class StatsUploadProvider : IStatsUploadService
    {
        private readonly IStatsUploadLoggingService loggingService;

        private readonly IStatsFileParserService statsFileParserService;

        private readonly IStatsUploadDataStoreService statsUploadDataStoreService;

        private readonly IStatsUploadEmailService statsUploadEmailService;

        public StatsUploadProvider(IStatsUploadDataStoreService statsUploadDataStoreService,
                                   IStatsUploadLoggingService loggingService,
                                   IStatsFileParserService statsFileParserService,
                                   IStatsUploadEmailService statsUploadEmailService)
        {
            this.statsUploadDataStoreService = statsUploadDataStoreService;
            this.loggingService = loggingService;
            this.statsFileParserService = statsFileParserService;
            this.statsUploadEmailService = statsUploadEmailService;
        }

        public StatsUploadResults UploadStatsFiles()
        {
            try
            {
                loggingService.LogVerbose($"{nameof(UploadStatsFiles)} Invoked");
                if (DataStoreUnavailable())
                {
                    return new StatsUploadResults(FailedReason.DataStoreUnavailable);
                }

                List<int> uploadFiles = statsUploadDataStoreService.GetDownloadsReadyForUpload();
                var statsUploadResults = new List<StatsUploadResult>();

                foreach (int downloadId in uploadFiles)
                {
                    UploadStatsFile(statsUploadResults, downloadId);
                }

                return new StatsUploadResults(statsUploadResults);
            }
            catch (Exception exception)
            {
                loggingService.LogException(exception);
                return new StatsUploadResults(FailedReason.UnexpectedException);
            }
        }

        private bool DataStoreUnavailable()
        {
            return !statsUploadDataStoreService.IsAvailable();
        }

        private void HandleFailedUsersData(List<FailedUserData> failedUsersData)
        {
            if (failedUsersData.Count > 0)
            {
                statsUploadEmailService.SendEmail(failedUsersData);
            }

            foreach (FailedUserData failedUserData in failedUsersData)
            {
                loggingService.LogException(failedUserData);
            }
        }

        private StatsUploadResult NewFailedStatsUploadResult(int downloadId, Exception exception)
        {
            if (exception is InvalidStatsFileException)
            {
                return new StatsUploadResult(downloadId, FailedReason.InvalidStatsFileUpload);
            }

            throw exception;
        }

        private void UploadStatsFile(List<StatsUploadResult> statsUploadResults, int downloadId)
        {
            try
            {
                loggingService.LogVerbose($"Starting stats file upload. DownloadId: {downloadId}");
                statsUploadDataStoreService.StartStatsUpload(downloadId);
                string fileData = statsUploadDataStoreService.GetFileData(downloadId);
                ParseResults results = statsFileParserService.Parse(fileData);
                List<UserData> usersData = results.UsersData;
                List<FailedUserData> failedUsersData = results.FailedUsersData;
                HandleFailedUsersData(failedUsersData);
                UploadUserData(usersData);
                statsUploadDataStoreService.StatsUploadFinished(downloadId);
                loggingService.LogVerbose($"Finished stats file upload. DownloadId: {downloadId}");
            }
            catch (Exception exception)
            {
                StatsUploadResult failedStatsUploadResult = NewFailedStatsUploadResult(downloadId, exception);
                loggingService.LogResult(failedStatsUploadResult);
                statsUploadEmailService.SendEmail(failedStatsUploadResult);
                statsUploadDataStoreService.StatsUploadError(failedStatsUploadResult);
                statsUploadResults.Add(failedStatsUploadResult);
            }
        }

        private void UploadUserData(List<UserData> usersData)
        {
            foreach (UserData userData in usersData)
            {
                statsUploadDataStoreService.AddUserData(userData);
            }
        }
    }
}