namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlTypes;
    using System.Linq;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public class TestHarnessStatsUploadDatabaseProvider : IStatsUploadDatabaseService
    {
        private readonly IStatsUploadDatabaseService innerService;

        private readonly TestHarnessSettings settings;

        public TestHarnessStatsUploadDatabaseProvider(IStatsUploadDatabaseService innerService,
                                                      IOptions<TestHarnessSettings> settings)
        {
            this.innerService = innerService;
            this.settings = settings.Value;
        }

        public void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
                             IList<FailedUserData> failedUsers)
        {
            innerService.AddUsers(transaction, downloadId, ModifiedUsers(usersData), failedUsers);
        }

        public void Commit(DbTransaction transaction)
        {
            innerService.Commit(transaction);
        }

        public DbTransaction CreateTransaction()
        {
            return innerService.CreateTransaction();
        }

        public IEnumerable<int> GetDownloadsReadyForUpload()
        {
            return innerService.GetDownloadsReadyForUpload();
        }

        public string GetFileData(int downloadId)
        {
            return innerService.GetFileData(downloadId);
        }

        public (bool isAvailable, FailedReason reason) IsAvailable()
        {
            return innerService.IsAvailable();
        }

        public void Rollback(DbTransaction transaction)
        {
            innerService.Rollback(transaction);
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

        private IEnumerable<UserData> ModifiedUsers(IEnumerable<UserData> usersData)
        {
            for (var index = 0; index < usersData.Count(); index++)
            {
                if (settings.EnableSqlExceptionDuringAddUsersTest && index == 2)
                {
                    throw new SqlTypeException("Mocking an exception being thrown during the add users.");
                }

                yield return usersData.ElementAt(index);
            }
        }
    }
}