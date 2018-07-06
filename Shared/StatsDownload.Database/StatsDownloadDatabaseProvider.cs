namespace StatsDownload.Database
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;
    using Core.Interfaces.Logging;

    public class StatsDownloadDatabaseProvider : IStatsDownloadDatabaseService
    {
        private const string DatabaseConnectionSuccessfulLogMessage = "Database connection was successful";

        private readonly IDatabaseConnectionServiceFactory databaseConnectionServiceFactory;

        private readonly IDatabaseConnectionSettingsService databaseConnectionSettingsService;

        private readonly IErrorMessageService errorMessageService;

        private readonly ILoggingService loggingService;

        public StatsDownloadDatabaseProvider(IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory,
            ILoggingService loggingService, IErrorMessageService errorMessageService)
        {
            this.databaseConnectionSettingsService = databaseConnectionSettingsService ??
                                                     throw new ArgumentNullException(
                                                         nameof(databaseConnectionSettingsService));
            this.databaseConnectionServiceFactory = databaseConnectionServiceFactory ??
                                                    throw new ArgumentNullException(
                                                        nameof(databaseConnectionServiceFactory));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.errorMessageService =
                errorMessageService ?? throw new ArgumentNullException(nameof(errorMessageService));
        }

        public void Commit(DbTransaction transaction)
        {
            transaction?.Commit();
        }

        public void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            string connectionString = GetConnectionString();
            int? commandTimeout = GetCommandTimeout();
            EnsureValidConnectionString(connectionString);
            IDatabaseConnectionService databaseConnection = CreateDatabaseConnection(connectionString, commandTimeout);
            EnsureDatabaseConnectionOpened(databaseConnection);
            action?.Invoke(databaseConnection);
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
            return databaseConnection.CreateParameter("@RejectionReason",
                DbType.String, ParameterDirection.Input);
        }

        public DbTransaction CreateTransaction()
        {
            loggingService.LogMethodInvoked();
            DbTransaction transaction = null;
            CreateDatabaseConnectionAndExecuteAction(service => { transaction = CreateTransaction(service); });
            return transaction;
        }

        public bool IsAvailable()
        {
            loggingService.LogMethodInvoked();

            try
            {
                CreateDatabaseConnectionAndExecuteAction(null);
                return true;
            }
            catch (Exception exception)
            {
                LogException(exception);
                return false;
            }
        }

        public void Rollback(DbTransaction transaction)
        {
            loggingService.LogMethodInvoked();
            transaction?.Rollback();
        }

        private IDatabaseConnectionService CreateDatabaseConnection(string connectionString, int? commandTimeout)
        {
            return databaseConnectionServiceFactory.Create(connectionString, commandTimeout);
        }

        private DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection, string message)
        {
            DbParameter errorMessage = databaseConnection.CreateParameter("@ErrorMessage", DbType.String,
                ParameterDirection.Input);
            errorMessage.Value = message;
            return errorMessage;
        }

        private DbTransaction CreateTransaction(IDatabaseConnectionService service)
        {
            return service.CreateTransaction();
        }

        private void EnsureDatabaseConnectionOpened(IDatabaseConnectionService databaseConnection)
        {
            if (databaseConnection.ConnectionState == ConnectionState.Closed)
            {
                databaseConnection.Open();
                LogVerbose(DatabaseConnectionSuccessfulLogMessage);
            }
        }

        private void EnsureValidConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Argument is empty", nameof(connectionString));
            }
        }

        private int? GetCommandTimeout()
        {
            return databaseConnectionSettingsService.GetCommandTimeout();
        }

        private string GetConnectionString()
        {
            return databaseConnectionSettingsService.GetConnectionString();
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }
    }
}