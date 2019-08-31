namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

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
            this.statsUploadDatabaseService = statsUploadDatabaseService
                                              ?? throw new ArgumentNullException(nameof(statsUploadDatabaseService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.statsFileParserService =
                statsFileParserService ?? throw new ArgumentNullException(nameof(statsFileParserService));
            this.statsUploadEmailService = statsUploadEmailService
                                           ?? throw new ArgumentNullException(nameof(statsUploadEmailService));
        }

        public StatsUploadResults UploadStatsFiles()
        {
            try
            {
                LogVerbose($"{nameof(UploadStatsFiles)} Invoked");
                if (DatabaseUnavailable(out FailedReason failedReason))
                {
                    var results = new StatsUploadResults(failedReason);
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

        private void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
                              IList<FailedUserData> failedUsersData)
        {
            statsUploadDatabaseService.AddUsers(transaction, downloadId, usersData, failedUsersData);
        }

        private bool DatabaseUnavailable(out FailedReason failedReason)
        {
            (bool isAvailable, FailedReason reason) = statsUploadDatabaseService.IsAvailable();
            failedReason = reason;
            return !isAvailable;
        }

        private IList<FailedUserData> GetEmailFailedUsers(IList<FailedUserData> failedUsersData)
        {
            //This is filtering out users that failed because of an unexpected format because this is a common occurance
            //Could consider removing if the file parsing engine is able to handle the common failing users
            //Or if these users are removed from the file
            return failedUsersData.Where(user => user.RejectionReason != RejectionReason.UnexpectedFormat).ToList();
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
            AddUsers(transaction, downloadId, usersData, failedUsersData);
            IList<FailedUserData> emailFailedUsers = GetEmailFailedUsers(failedUsersData);
            SendFailedUsers(emailFailedUsers);
            LogFailedUserData(downloadId, failedUsersData);
        }

        private void LogFailedUserData(int downloadId, IList<FailedUserData> failedUsersData)
        {
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

        private void SendFailedUsers(IList<FailedUserData> emailFailedUsers)
        {
            if (emailFailedUsers.Any())
            {
                statsUploadEmailService.SendEmail(emailFailedUsers);
            }
        }

        private void UploadStatsFile(List<StatsUploadResult> statsUploadResults, int downloadId)
        {
            DbTransaction transaction = null;

            try
            {
                LogVerbose($"Starting stats file upload. DownloadId: {downloadId}");
                string fileData = statsUploadDatabaseService.GetFileData(downloadId);
                ParseResults results =
                    statsFileParserService.Parse(new FilePayload { DecompressedDownloadFileData = fileData });
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