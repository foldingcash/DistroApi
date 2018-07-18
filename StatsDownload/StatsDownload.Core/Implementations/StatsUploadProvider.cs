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
            this.statsUploadDatabaseService = statsUploadDatabaseService ??
                                              throw new ArgumentNullException(nameof(statsUploadDatabaseService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.statsFileParserService = statsFileParserService ??
                                          throw new ArgumentNullException(nameof(statsFileParserService));
            this.statsUploadEmailService = statsUploadEmailService ??
                                           throw new ArgumentNullException(nameof(statsUploadEmailService));
        }

        public StatsUploadResults UploadStatsFiles()
        {
            try
            {
                LogVerbose($"{nameof(UploadStatsFiles)} Invoked");
                if (DatabaseUnavailable())
                {
                    var results = new StatsUploadResults(FailedReason.DatabaseUnavailable);
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

        private bool DatabaseUnavailable()
        {
            return !statsUploadDatabaseService.IsAvailable();
        }

        private FailedReason GetFailedReason(Exception exception)
        {
            if (exception is InvalidStatsFileException)
            {
                return FailedReason.InvalidStatsFileUpload;
            }

            if (exception is DbException)
            {
                return FailedReason.UnexpectedDatabaseException;
            }

            return FailedReason.UnexpectedException;
        }

        private void HandleUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
            IList<FailedUserData> failedUsersData)
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

        private StatsUploadResult NewFailedStatsUploadResult(int downloadId, FailedReason failedReason)
        {
            return new StatsUploadResult(downloadId, failedReason);
        }

        private void UploadStatsFile(List<StatsUploadResult> statsUploadResults, int downloadId)
        {
            DbTransaction transaction = null;

            try
            {
                LogVerbose($"Starting stats file upload. DownloadId: {downloadId}");
                string fileData = statsUploadDatabaseService.GetFileData(downloadId);
                ParseResults results = statsFileParserService.Parse(fileData);
                transaction = statsUploadDatabaseService.CreateTransaction();
                statsUploadDatabaseService.StartStatsUpload(transaction, downloadId, results.DownloadDateTime);
                IEnumerable<UserData> usersData = results.UsersData;
                IList<FailedUserData> failedUsersData = results.FailedUsersData.ToList();
                HandleUsers(transaction, downloadId, usersData, failedUsersData);
                statsUploadDatabaseService.StatsUploadFinished(transaction, downloadId);
                statsUploadDatabaseService.Commit(transaction);
                LogVerbose($"Finished stats file upload. DownloadId: {downloadId}");
            }
            catch (Exception exception)
            {
                statsUploadDatabaseService.Rollback(transaction);
                FailedReason failedReason = GetFailedReason(exception);
                StatsUploadResult failedStatsUploadResult = NewFailedStatsUploadResult(downloadId, failedReason);
                loggingService.LogResult(failedStatsUploadResult);
                loggingService.LogException(exception);
                statsUploadEmailService.SendEmail(failedStatsUploadResult);
                statsUploadDatabaseService.StatsUploadError(failedStatsUploadResult);
                statsUploadResults.Add(failedStatsUploadResult);
            }
        }
    }
}