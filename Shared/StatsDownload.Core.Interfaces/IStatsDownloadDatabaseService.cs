namespace StatsDownload.Core.Interfaces
{
    using System;
    using System.Data.Common;

    public interface IStatsDownloadDatabaseService
    {
        void Commit(DbTransaction transaction);

        void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action);

        DbTransaction CreateTransaction();

        bool IsAvailable();

        void Rollback(DbTransaction transaction);
    }
}