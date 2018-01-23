namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using StatsDownload.Logging;

    public class FileDownloadDataStoreProvider : IFileDownloadDataStoreService
    {
        private const string DatabaseConnectionSuccessfulLogMessage = "Database connection was successful";

        private const string FileDownloadFinishedProcedureName = "[FoldingCoin].[FileDownloadFinished]";

        private readonly IDatabaseConnectionServiceFactory databaseConnectionServiceFactory;

        private readonly IDatabaseConnectionSettingsService databaseConnectionSettingsService;

        private readonly ILoggingService loggingService;

        private readonly string NewFileDownloadStartedProcedureName = "[FoldingCoin].[NewFileDownloadStarted]";

        private readonly string UpdateToLatestStoredProcedureName = "[FoldingCoin].[UpdateToLatest]";

        public FileDownloadDataStoreProvider(IDatabaseConnectionSettingsService databaseConnectionSettingsService,
                                             IDatabaseConnectionServiceFactory databaseConnectionServiceFactory,
                                             ILoggingService loggingService)
        {
            if (IsNull(databaseConnectionSettingsService))
            {
                throw NewArgumentNullException(nameof(databaseConnectionSettingsService));
            }

            if (IsNull(databaseConnectionServiceFactory))
            {
                throw NewArgumentNullException(nameof(databaseConnectionServiceFactory));
            }

            if (IsNull(loggingService))
            {
                throw NewArgumentNullException(nameof(loggingService));
            }

            this.databaseConnectionSettingsService = databaseConnectionSettingsService;
            this.databaseConnectionServiceFactory = databaseConnectionServiceFactory;
            this.loggingService = loggingService;
        }

        public void FileDownloadFinished(FilePayload filePayload)
        {
            LogMethodInvoked(nameof(FileDownloadFinished));
            CreateDatabaseConnectionAndExecuteAction(service => { FileDownloadFinished(service, filePayload); });
        }

        public DateTime GetLastFileDownloadDateTime()
        {
            LogMethodInvoked(nameof(GetLastFileDownloadDateTime));
            DateTime lastFileDownloadDateTime = default(DateTime);
            CreateDatabaseConnectionAndExecuteAction(
                service => { lastFileDownloadDateTime = GetLastFileDownloadDateTime(service); });
            return lastFileDownloadDateTime;
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

        public void NewFileDownloadStarted(FilePayload filePayload)
        {
            LogMethodInvoked(nameof(NewFileDownloadStarted));
            int downloadId = default(int);
            CreateDatabaseConnectionAndExecuteAction(service => { downloadId = NewFileDownloadStarted(service); });
            filePayload.DownloadId = downloadId;
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
                EnsureValidConnectionString(connectionString);
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

        private void FileDownloadFinished(IDatabaseConnectionService databaseConnection, FilePayload filePayload)
        {
            DbParameter downloadId = databaseConnection.CreateParameter("@DownloadId", DbType.Int32,
                ParameterDirection.Input);
            downloadId.Value = filePayload.DownloadId;

            DbParameter fileName = databaseConnection.CreateParameter("@FileName", DbType.String,
                ParameterDirection.Input);
            fileName.Value = filePayload.DecompressedDownloadFileName;

            DbParameter fileExtension = databaseConnection.CreateParameter("@FileExtension", DbType.String,
                ParameterDirection.Input);
            fileExtension.Value = filePayload.DecompressedDownloadFileExtension;

            DbParameter fileData = databaseConnection.CreateParameter("@FileData", DbType.String,
                ParameterDirection.Input);
            fileData.Value = filePayload.DecompressedDownloadFileData;

            databaseConnection.ExecuteStoredProcedure(FileDownloadFinishedProcedureName,
                new List<DbParameter> { downloadId, fileName, fileExtension, fileData });
        }

        private string GetConnectionString()
        {
            return databaseConnectionSettingsService.GetConnectionString();
        }

        private DateTime GetLastFileDownloadDateTime(IDatabaseConnectionService service)
        {
            return service.ExecuteScalar("SELECT [FoldingCoin].[GetLastFileDownloadDateTime]()") as DateTime?
                   ?? default(DateTime);
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogMethodInvoked(string method)
        {
            LogVerbose($"{method} Invoked");
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private int NewFileDownloadStarted(IDatabaseConnectionService databaseConnection)
        {
            DbParameter downloadId = databaseConnection.CreateParameter("@DownloadId", DbType.Int32,
                ParameterDirection.Output);

            databaseConnection.ExecuteStoredProcedure(NewFileDownloadStartedProcedureName,
                new List<DbParameter> { downloadId });

            return (int)downloadId.Value;
        }

        private void OpenDatabaseConnection(IDatabaseConnectionService databaseConnectionService)
        {
            databaseConnectionService.Open();
        }

        private void UpdateToLatest(IDatabaseConnectionService databaseConnection)
        {
            int numberOfRowsEffected = databaseConnection.ExecuteStoredProcedure(UpdateToLatestStoredProcedureName);

            LogVerbose($"'{numberOfRowsEffected}' rows were effected");
        }
    }
}