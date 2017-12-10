namespace StatsDownload.Core
{
    using System;

    public class FileDownloadDataStoreProvider : IFileDownloadDataStoreService
    {
        private const string DatabaseConnectionSuccessfulLogMessage = "Database connection was successful";

        private readonly IDatabaseConnectionServiceFactory databaseConnectionServiceFactory;

        private readonly IDatabaseConnectionSettingsService databaseConnectionSettingsService;

        private readonly IFileDownloadLoggingService fileDownloadLoggingService;

        private readonly string UpdateToLatestStoredProcedureName = "[FoldingCoin].[UpdateToLatest]";

        public FileDownloadDataStoreProvider(
            IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory,
            IFileDownloadLoggingService fileDownloadLoggingService)
        {
            if (IsNull(databaseConnectionSettingsService))
            {
                throw NewArgumentNullException(nameof(databaseConnectionSettingsService));
            }

            if (IsNull(databaseConnectionServiceFactory))
            {
                throw NewArgumentNullException(nameof(databaseConnectionServiceFactory));
            }

            if (IsNull(fileDownloadLoggingService))
            {
                throw NewArgumentNullException(nameof(fileDownloadLoggingService));
            }

            this.databaseConnectionSettingsService = databaseConnectionSettingsService;
            this.databaseConnectionServiceFactory = databaseConnectionServiceFactory;
            this.fileDownloadLoggingService = fileDownloadLoggingService;
        }

        public bool IsAvailable()
        {
            LogMethodInvoked(nameof(IsAvailable));

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

        public void UpdateToLatest()
        {
            LogMethodInvoked(nameof(UpdateToLatest));
            CreateDatabaseConnectionAndExecuteAction(UpdateToLatest);
        }

        private void CloseDatabaseConnection(IDatabaseConnectionService databaseConnection)
        {
            databaseConnection?.Close();
        }

        private IDatabaseConnectionService CreateDatabaseConnection(string connectionString)
        {
            return databaseConnectionServiceFactory.Create(connectionString);
        }

        private void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            IDatabaseConnectionService databaseConnection = default(IDatabaseConnectionService);
            try
            {
                string connectionString = GetConnectionString();
                databaseConnection = CreateDatabaseConnection(connectionString);
                OpenDatabaseConnection(databaseConnection);
                LogVerbose(DatabaseConnectionSuccessfulLogMessage);
                action?.Invoke(databaseConnection);
            }
            finally
            {
                CloseDatabaseConnection(databaseConnection);
            }
        }

        private string GetConnectionString()
        {
            return databaseConnectionSettingsService.GetConnectionString();
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private void LogException(Exception exception)
        {
            fileDownloadLoggingService.LogException(exception);
        }

        private void LogMethodInvoked(string method)
        {
            LogVerbose($"{method} Invoked");
        }

        private void LogVerbose(string message)
        {
            fileDownloadLoggingService.LogVerbose(message);
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private void OpenDatabaseConnection(IDatabaseConnectionService databaseConnectionService)
        {
            databaseConnectionService.Open();
        }

        private void UpdateToLatest(IDatabaseConnectionService databaseConnection)
        {
            databaseConnection.ExecuteStoredProcedure(UpdateToLatestStoredProcedureName);
        }
    }
}