namespace StatsDownload.Core.Interfaces
{
    using System;
    using System.Data;
    using System.Data.Common;
    using DataTransfer;

    public interface IStatsDownloadDatabaseService
    {
        void Commit(DbTransaction transaction);

        void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action);

        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection, int downloadId);

        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection);

        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection,
            ParameterDirection direction);

        DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
            StatsUploadResult statsUploadResult);

        DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
            FileDownloadResult fileDownloadResult);

        DbParameter CreateRejectionReasonParameter(IDatabaseConnectionService databaseConnection);

        DbTransaction CreateTransaction();

        bool IsAvailable();

        void Rollback(DbTransaction transaction);
    }
}