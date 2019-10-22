namespace StatsDownload.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IStatsUploadDatabaseService
    {
        void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> usersData,
                      IList<FailedUserData> failedUsers);

        void Commit(DbTransaction transaction);

        DbTransaction CreateTransaction();

        IEnumerable<int> GetDownloadsReadyForUpload();

        string GetFileData(int downloadId);

        (bool isAvailable, FailedReason reason) IsAvailable();

        void Rollback(DbTransaction transaction);

        void StartStatsUpload(DbTransaction transaction, int downloadId, DateTime downloadDateTime);

        void StatsUploadError(StatsUploadResult statsUploadResult);

        void StatsUploadFinished(DbTransaction transaction, int downloadId);
    }
}