namespace StatsDownload.TestHarness
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlTypes;
    using System.Linq;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;

    public class TestHarnessStatsDownloadDatabaseProvider : IStatsDownloadDatabaseService
    {
        private readonly IStatsDownloadDatabaseService innerService;

        private readonly ITestHarnessStatsDownloadSettings settings;

        public TestHarnessStatsDownloadDatabaseProvider(IStatsDownloadDatabaseService innerService,
            ITestHarnessStatsDownloadSettings settings)
        {
            this.innerService = innerService;
            this.settings = settings;
        }

        public DbTransaction CreateTransaction()
        {
            return innerService.CreateTransaction();
        }

        public void Rollback(DbTransaction transaction)
        {
            innerService.Rollback(transaction);
        }

        public void Commit(DbTransaction transaction)
        {
            innerService.Commit(transaction);
        }

        public void FileDownloadError(FileDownloadResult fileDownloadResult)
        {
            innerService.FileDownloadError(fileDownloadResult);
        }

        public void FileDownloadFinished(FilePayload filePayload)
        {
            innerService.FileDownloadFinished(filePayload);
        }

        public DateTime GetLastFileDownloadDateTime()
        {
            return innerService.GetLastFileDownloadDateTime();
        }

        public bool IsAvailable()
        {
            return innerService.IsAvailable();
        }

        public void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
            IList<FailedUserData> failedUsers)
        {
            innerService.AddUsers(transaction, downloadId, ModifiedUsers(usersData), failedUsers);
        }

        public IEnumerable<int> GetDownloadsReadyForUpload()
        {
            return innerService.GetDownloadsReadyForUpload();
        }

        public string GetFileData(int downloadId)
        {
            return innerService.GetFileData(downloadId);
        }

        public void StartStatsUpload(DbTransaction transaction, int downloadId, DateTime downloadDateTime)
        {
            innerService.StartStatsUpload(transaction, downloadId, downloadDateTime);
        }

        public void StatsUploadError(StatsUploadResult statsUploadResult)
        {
            innerService.StatsUploadError(statsUploadResult);
        }

        public void StatsUploadFinished(DbTransaction transaction, int downloadId)
        {
            innerService.StatsUploadFinished(transaction, downloadId);
        }

        public void NewFileDownloadStarted(FilePayload filePayload)
        {
            innerService.NewFileDownloadStarted(filePayload);
        }

        public void UpdateToLatest()
        {
            innerService.UpdateToLatest();
        }

        private IEnumerable<UserData> ModifiedUsers(IEnumerable<UserData> usersData)
        {
            for (var index = 0; index < usersData.Count(); index++)
            {
                if (settings.Enabled && index == 2)
                {
                    throw new SqlTypeException("Mocking an exception being thrown during the add users.");
                }

                yield return usersData.ElementAt(index);
            }
        }
    }
}