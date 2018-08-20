namespace StatsDownload.Core.Interfaces
{
    using System;
    using System.Data.Common;
    using Enums;

    public interface IStatsDownloadDatabaseService
    {
        void Commit(DbTransaction transaction);

        void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action);

        DbTransaction CreateTransaction();

        (bool isAvailable, DatabaseFailedReason reason) IsAvailable(string[] requiredObjects);

        void Rollback(DbTransaction transaction);
    }
}