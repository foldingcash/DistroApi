namespace StatsDownload.Database
{
    using System;
    using System.Data;
    using System.Data.Common;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public class StatsDownloadDatabaseParameterProvider : IStatsDownloadDatabaseParameterService
    {
        private readonly IErrorMessageService errorMessageService;

        public StatsDownloadDatabaseParameterProvider(IErrorMessageService errorMessageService)
        {
            this.errorMessageService =
                errorMessageService ?? throw new ArgumentNullException(nameof(errorMessageService));
        }

        public DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            DbParameter downloadIdParameter = CreateDownloadIdParameter(databaseConnection);
            downloadIdParameter.Value = downloadId;
            return downloadIdParameter;
        }

        public DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection)
        {
            return databaseConnection.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Input);
        }

        public DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection,
                                                     ParameterDirection direction)
        {
            DbParameter downloadIdParameter = CreateDownloadIdParameter(databaseConnection);
            downloadIdParameter.Direction = direction;
            return downloadIdParameter;
        }

        public DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
                                                       FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;
            string message = errorMessageService.GetErrorMessage(fileDownloadResult.FailedReason, filePayload,
                StatsDownloadService.FileDownload);
            return CreateErrorMessageParameter(databaseConnection, message);
        }

        public DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
                                                       StatsUploadResult statsUploadResult)
        {
            string message =
                errorMessageService.GetErrorMessage(statsUploadResult.FailedReason, StatsDownloadService.StatsUpload);
            return CreateErrorMessageParameter(databaseConnection, message);
        }

        public DbParameter CreateRejectionReasonParameter(IDatabaseConnectionService databaseConnection)
        {
            return databaseConnection.CreateParameter("@RejectionReason", DbType.String, ParameterDirection.Input);
        }

        private DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection, string message)
        {
            DbParameter errorMessage = databaseConnection.CreateParameter("@ErrorMessage", DbType.String,
                ParameterDirection.Input);
            errorMessage.Value = message;
            return errorMessage;
        }
    }
}