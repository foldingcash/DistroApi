namespace StatsDownload.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Logging;

    public class FileDownloadDatabaseProvider : IFileDownloadDatabaseService
    {
        private readonly ILoggingService loggingService;

        private readonly IStatsDownloadDatabaseService statsDownloadDatabaseService;

        public FileDownloadDatabaseProvider(IStatsDownloadDatabaseService statsDownloadDatabaseService,
            ILoggingService loggingService)
        {
            this.statsDownloadDatabaseService = statsDownloadDatabaseService ??
                                                throw new ArgumentNullException(nameof(statsDownloadDatabaseService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public void FileDownloadError(FileDownloadResult fileDownloadResult)
        {
            loggingService.LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => { FileDownloadError(service, fileDownloadResult); });
        }

        public void FileDownloadFinished(FilePayload filePayload)
        {
            loggingService.LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => { FileDownloadFinished(service, filePayload); });
        }

        public DateTime GetLastFileDownloadDateTime()
        {
            loggingService.LogMethodInvoked();
            DateTime lastFileDownloadDateTime = default(DateTime);
            CreateDatabaseConnectionAndExecuteAction(
                service => { lastFileDownloadDateTime = GetLastFileDownloadDateTime(service); });
            return lastFileDownloadDateTime;
        }

        public bool IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable();
        }

        public void NewFileDownloadStarted(FilePayload filePayload)
        {
            loggingService.LogMethodInvoked();
            int downloadId = default(int);
            CreateDatabaseConnectionAndExecuteAction(service => { downloadId = NewFileDownloadStarted(service); });
            filePayload.DownloadId = downloadId;
        }

        public void UpdateToLatest()
        {
            loggingService.LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(UpdateToLatest);
        }

        private void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(action);
        }

        private DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            return statsDownloadDatabaseService.CreateDownloadIdParameter(databaseConnection, downloadId);
        }

        private void FileDownloadError(IDatabaseConnectionService databaseConnection,
            FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;

            DbParameter downloadId = CreateDownloadIdParameter(databaseConnection, filePayload.DownloadId);

            DbParameter errorMessage =
                statsDownloadDatabaseService.CreateErrorMessageParameter(databaseConnection, fileDownloadResult);

            databaseConnection.ExecuteStoredProcedure(Constants.StatsDownloadDatabase.FileDownloadErrorProcedureName,
                new List<DbParameter> { downloadId, errorMessage });
        }

        private void FileDownloadFinished(IDatabaseConnectionService databaseConnection, FilePayload filePayload)
        {
            DbParameter downloadId = CreateDownloadIdParameter(databaseConnection, filePayload.DownloadId);

            DbParameter fileName = databaseConnection.CreateParameter("@FileName", DbType.String,
                ParameterDirection.Input);
            fileName.Value = filePayload.DecompressedDownloadFileName;

            DbParameter fileExtension = databaseConnection.CreateParameter("@FileExtension", DbType.String,
                ParameterDirection.Input);
            fileExtension.Value = filePayload.DecompressedDownloadFileExtension;

            DbParameter fileData = databaseConnection.CreateParameter("@FileData", DbType.String,
                ParameterDirection.Input);
            fileData.Value = filePayload.DecompressedDownloadFileData;

            databaseConnection.ExecuteStoredProcedure(
                Constants.StatsDownloadDatabase.FileDownloadFinishedProcedureName,
                new List<DbParameter> { downloadId, fileName, fileExtension, fileData });
        }

        private DateTime GetLastFileDownloadDateTime(IDatabaseConnectionService databaseConnection)
        {
            return
                databaseConnection.ExecuteScalar(Constants.StatsDownloadDatabase.GetLastFileDownloadDateTimeSql) as
                    DateTime? ?? default(DateTime);
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private int NewFileDownloadStarted(IDatabaseConnectionService databaseConnection)
        {
            DbParameter downloadId =
                statsDownloadDatabaseService.CreateDownloadIdParameter(databaseConnection, ParameterDirection.Output);

            databaseConnection.ExecuteStoredProcedure(
                Constants.StatsDownloadDatabase.NewFileDownloadStartedProcedureName,
                new List<DbParameter> { downloadId });

            if (downloadId?.Value == null || downloadId.Value == DBNull.Value)
            {
                throw new ArgumentNullException();
            }

            return (int) downloadId.Value;
        }

        private void UpdateToLatest(IDatabaseConnectionService databaseConnection)
        {
            int numberOfRowsEffected =
                databaseConnection.ExecuteStoredProcedure(
                    Constants.StatsDownloadDatabase.UpdateToLatestStoredProcedureName);

            LogVerbose($"'{numberOfRowsEffected}' rows were effected");
        }
    }
}