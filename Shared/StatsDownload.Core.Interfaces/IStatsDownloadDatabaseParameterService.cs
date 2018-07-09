namespace StatsDownload.Core.Interfaces
{
    using System.Data;
    using System.Data.Common;
    using DataTransfer;

    public interface IStatsDownloadDatabaseParameterService
    {
        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnectionService,
            ParameterDirection direction);

        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnectionService);

        DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnectionService, int downloadId);

        DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnectionService,
            FileDownloadResult fileDownloadResult);

        DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnectionService,
            StatsUploadResult statsUploadResult);

        DbParameter CreateRejectionReasonParameter(IDatabaseConnectionService databaseConnectionService);
    }
}