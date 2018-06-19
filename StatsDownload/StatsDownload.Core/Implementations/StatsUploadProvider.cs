namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Exceptions;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;

    public class StatsUploadProvider : IStatsUploadService
    {
        private readonly IStatsUploadLoggingService loggingService;

        private readonly IStatsFileParserService statsFileParserService;

        private readonly IStatsUploadDatabaseService statsUploadDatabaseService;

        private readonly IStatsUploadEmailService statsUploadEmailService;

        public StatsUploadProvider(IStatsUploadDatabaseService statsUploadDatabaseService,
            IStatsUploadLoggingService loggingService,
            IStatsFileParserService statsFileParserService,
            IStatsUploadEmailService statsUploadEmailService)
        {
            if (statsUploadDatabaseService == null)
            {
                throw new ArgumentNullException(nameof(statsUploadDatabaseService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            if (statsFileParserService == null)
            {
                throw new ArgumentNullException(nameof(statsFileParserService));
            }

            if (statsUploadEmailService == null)
            {
                throw new ArgumentNullException(nameof(statsUploadEmailService));
            }

            this.statsUploadDatabaseService = statsUploadDatabaseService;
            this.loggingService = loggingService;
            this.statsFileParserService = statsFileParserService;
            this.statsUploadEmailService = statsUploadEmailService;
        }

        public StatsUploadResults UploadStatsFiles()
        {
            try
            {
                LogVerbose($"{nameof(UploadStatsFiles)} Invoked");
                if (DataStoreUnavailable())
                {
                    var results = new StatsUploadResults(FailedReason.DataStoreUnavailable);
                    loggingService.LogResults(results);
                    statsUploadEmailService.SendEmail(results);
                    return results;
                }

                List<int> uploadFiles = statsUploadDatabaseService.GetDownloadsReadyForUpload().ToList();
                var statsUploadResults = new List<StatsUploadResult>();

                foreach (int downloadId in uploadFiles)
                {
                    UploadStatsFile(statsUploadResults, downloadId);
                }

                return new StatsUploadResults(statsUploadResults);
            }
            catch (Exception exception)
            {
                var results = new StatsUploadResults(FailedReason.UnexpectedException);
                loggingService.LogException(exception);
                statsUploadEmailService.SendEmail(results);
                return results;
            }
        }

        private bool DataStoreUnavailable()
        {
            return !statsUploadDatabaseService.IsAvailable();
        }

        private FailedReason GetFailedReason(Exception exception)
        {
            if (exception is InvalidStatsFileException)
            {
                return FailedReason.InvalidStatsFileUpload;
            }

            return FailedReason.UnexpectedException;
        }

        private void HandleUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
            IEnumerable<FailedUserData> failedUsersData)
        {
            statsUploadDatabaseService.AddUsers(transaction, downloadId, usersData, failedUsersData);

            if (failedUsersData.Any())
            {
                statsUploadEmailService.SendEmail(failedUsersData);
            }

            foreach (FailedUserData failedUserData in failedUsersData)
            {
                loggingService.LogFailedUserData(downloadId, failedUserData);
            }
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private StatsUploadResult NewFailedStatsUploadResult(int downloadId, FailedReason failedReason,
            Exception exception)
        {
            return new StatsUploadResult(downloadId, failedReason, exception);
        }

        private void UploadStatsFile(List<StatsUploadResult> statsUploadResults, int downloadId)
        {
            DbTransaction transaction = null;

            try
            {
                LogVerbose($"Starting stats file upload. DownloadId: {downloadId}");
                string fileData = statsUploadDatabaseService.GetFileData(downloadId);
                transaction = statsUploadDatabaseService.StartStatsUpload(downloadId);
                ParseResults results = statsFileParserService.Parse(fileData);
                IEnumerable<UserData> usersData = results.UsersData;
                IEnumerable<FailedUserData> failedUsersData = results.FailedUsersData;
                HandleUsers(transaction, downloadId, usersData, failedUsersData);
                statsUploadDatabaseService.StatsUploadFinished(transaction, downloadId, results.DownloadDateTime);
                statsUploadDatabaseService.Commit(transaction);
                LogVerbose($"Finished stats file upload. DownloadId: {downloadId}");
            }
            catch (Exception exception)
            {
                statsUploadDatabaseService.Rollback(transaction);
                FailedReason failedReason = GetFailedReason(exception);
                StatsUploadResult failedStatsUploadResult =
                    NewFailedStatsUploadResult(downloadId, failedReason, exception);
                loggingService.LogResult(failedStatsUploadResult);
                statsUploadEmailService.SendEmail(failedStatsUploadResult);
                statsUploadDatabaseService.StatsUploadError(failedStatsUploadResult);
                statsUploadResults.Add(failedStatsUploadResult);
            }
        }
    }
}