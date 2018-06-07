namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Logging;

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
            if (databaseConnectionSettingsService == null)
            {
                throw new ArgumentNullException(nameof(databaseConnectionSettingsService));
            }

            if (databaseConnectionServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(databaseConnectionServiceFactory));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            if (errorMessageService == null)
            {
                throw new ArgumentNullException(nameof(errorMessageService));
            }

            this.databaseConnectionSettingsService = databaseConnectionSettingsService;
            this.databaseConnectionServiceFactory = databaseConnectionServiceFactory;
            this.loggingService = loggingService;
            this.errorMessageService = errorMessageService;
        }

        public void AddUsers(int downloadId, IEnumerable<UserData> users, IEnumerable<FailedUserData> failedUsers)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => { AddUsers(service, downloadId, users, failedUsers); });
        }

        public void FileDownloadError(FileDownloadResult fileDownloadResult)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => { FileDownloadError(service, fileDownloadResult); });
        }

        public void FileDownloadFinished(FilePayload filePayload)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => { FileDownloadFinished(service, filePayload); });
        }

        public IEnumerable<int> GetDownloadsReadyForUpload()
        {
            LogMethodInvoked();
            List<int> downloadsReadyForUpload = default(List<int>);
            CreateDatabaseConnectionAndExecuteAction(
                service => downloadsReadyForUpload = GetDownloadsReadyForUpload(service));
            return downloadsReadyForUpload;
        }

        public string GetFileData(int downloadId)
        {
            LogMethodInvoked();
            string fileData = default(string);
            CreateDatabaseConnectionAndExecuteAction(service => fileData = GetFileData(service, downloadId));
            return fileData;
        }

        public DateTime GetLastFileDownloadDateTime()
        {
            LogMethodInvoked();
            DateTime lastFileDownloadDateTime = default(DateTime);
            CreateDatabaseConnectionAndExecuteAction(
                service => { lastFileDownloadDateTime = GetLastFileDownloadDateTime(service); });
            return lastFileDownloadDateTime;
        }

        public bool IsAvailable()
        {
            LogMethodInvoked();

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
            LogMethodInvoked();
            int downloadId = default(int);
            CreateDatabaseConnectionAndExecuteAction(service => { downloadId = NewFileDownloadStarted(service); });
            filePayload.DownloadId = downloadId;
        }

        public void StartStatsUpload(int downloadId)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => StartStatsUpload(service, downloadId));
        }

        public void StatsUploadError(StatsUploadResult statsUploadResult)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => StatsUploadError(service, statsUploadResult));
        }

        public void StatsUploadFinished(int downloadId)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => StatsUploadFinished(service, downloadId));
        }

        public void UpdateToLatest()
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(UpdateToLatest);
        }

        private void AddUserRejections(IDatabaseConnectionService databaseConnection, int downloadId,
            IEnumerable<FailedUserData> failedUsersData)
        {
            AddUserRejectionParameters parameters = CreateAddUserRejectionParameters(databaseConnection);

            using (DbCommand command = databaseConnection.CreateDbCommand())
            {
                command.CommandText = Constants.StatsDownloadDatabase.AddUserRejectionProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters.AllParameters);

                foreach (FailedUserData failedUserData in failedUsersData ?? new FailedUserData[0])
                {
                    SetAddUserRejectionParameters(downloadId, failedUserData, parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddUsersData(IDatabaseConnectionService databaseConnection, int downloadId,
            IEnumerable<UserData> usersData)
        {
            AddUserDataParameters parameters = CreateAddUserDataParameters(databaseConnection);

            using (DbCommand addUserDataCommand = databaseConnection.CreateDbCommand())
            {
                using (DbCommand rebuildIndicesCommand = databaseConnection.CreateDbCommand())
                {
                    addUserDataCommand.CommandText = Constants.StatsDownloadDatabase.AddUserDataProcedureName;
                    addUserDataCommand.CommandType = CommandType.StoredProcedure;
                    addUserDataCommand.Parameters.AddRange(parameters.AllParameters);

                    rebuildIndicesCommand.CommandText = Constants.StatsDownloadDatabase.RebuildIndicesProcedureName;
                    rebuildIndicesCommand.CommandType = CommandType.StoredProcedure;

                    for (var index = 0; index < (usersData?.Count() ?? 0); index++)
                    {
                        UserData userData = usersData.ElementAt(index);
                        SetAddUserDataParameterValues(downloadId, userData, parameters);
                        addUserDataCommand.ExecuteNonQuery();

                        if (index % 2500 == 0)
                        {
                            rebuildIndicesCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void AddUsers(IDatabaseConnectionService databaseConnection, int downloadId,
            IEnumerable<UserData> users, IEnumerable<FailedUserData> failedUsers)
        {
            var transaction = databaseConnection.CreateTransaction();

            try
            {
                AddUserRejections(databaseConnection, downloadId, failedUsers);
                AddUsersData(databaseConnection, downloadId, users);
                transaction?.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
        }

        private AddUserDataParameters CreateAddUserDataParameters(IDatabaseConnectionService databaseConnection)
        {
            return new AddUserDataParameters
            {
                DownloadId = CreateDownloadIdParameter(databaseConnection),
                FahUserName =
                    databaseConnection.CreateParameter("@FAHUserName", DbType.String,
                        ParameterDirection.Input),
                TotalPoints =
                    databaseConnection.CreateParameter("@TotalPoints", DbType.Int64,
                        ParameterDirection.Input),
                WorkUnits =
                    databaseConnection.CreateParameter("@WorkUnits", DbType.Int64,
                        ParameterDirection.Input),
                TeamNumber =
                    databaseConnection.CreateParameter("@TeamNumber", DbType.Int64,
                        ParameterDirection.Input),
                FriendlyName =
                    databaseConnection.CreateParameter("@FriendlyName", DbType.String,
                        ParameterDirection.Input),
                BitcoinAddress =
                    databaseConnection.CreateParameter("@BitcoinAddress", DbType.String,
                        ParameterDirection.Input)
            };
        }

        private AddUserRejectionParameters CreateAddUserRejectionParameters(
            IDatabaseConnectionService databaseConnection)
        {
            return new AddUserRejectionParameters
            {
                DownloadId = CreateDownloadIdParameter(databaseConnection),
                LineNumber =
                    databaseConnection.CreateParameter("@LineNumber", DbType.Int32,
                        ParameterDirection.Input),
                RejectionReason =
                    databaseConnection.CreateParameter("@RejectionReason",
                        DbType.String, ParameterDirection.Input)
            };
        }

        private IDatabaseConnectionService CreateDatabaseConnection(string connectionString)
        {
            return databaseConnectionServiceFactory.Create(connectionString);
        }

        private void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            string connectionString = GetConnectionString();
            EnsureValidConnectionString(connectionString);
            IDatabaseConnectionService databaseConnection = CreateDatabaseConnection(connectionString);
            EnsureDatabaseConnectionOpened(databaseConnection);
            action?.Invoke(databaseConnection);
        }

        private DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            DbParameter downloadIdParameter = CreateDownloadIdParameter(databaseConnection);
            downloadIdParameter.Value = downloadId;
            return downloadIdParameter;
        }

        private DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection)
        {
            return databaseConnection.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Input);
        }

        private DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
            FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;
            string message = errorMessageService.GetErrorMessage(fileDownloadResult.FailedReason, filePayload);
            return CreateErrorMessageParameter(databaseConnection, message);
        }

        private DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection, string message)
        {
            DbParameter errorMessage = databaseConnection.CreateParameter("@ErrorMessage", DbType.String,
                ParameterDirection.Input);
            errorMessage.Value = message;
            return errorMessage;
        }

        private DbParameter CreateErrorMessageParameter(IDatabaseConnectionService databaseConnection,
            StatsUploadResult fileDownloadResult)
        {
            string message = errorMessageService.GetErrorMessage(fileDownloadResult.FailedReason);
            return CreateErrorMessageParameter(databaseConnection, message);
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

        private void ExecuteStoredProcedure(IDatabaseConnectionService databaseConnection, string storedProcedure,
            List<DbParameter> parameters)
        {
            databaseConnection.ExecuteStoredProcedure(storedProcedure, parameters);
        }

        private void FileDownloadError(IDatabaseConnectionService databaseConnection,
            FileDownloadResult fileDownloadResult)
        {
            FilePayload filePayload = fileDownloadResult.FilePayload;

            DbParameter downloadId = CreateDownloadIdParameter(databaseConnection, filePayload.DownloadId);

            DbParameter errorMessage = CreateErrorMessageParameter(databaseConnection, fileDownloadResult);

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

        private string GetConnectionString()
        {
            return databaseConnectionSettingsService.GetConnectionString();
        }

        private List<int> GetDownloadsReadyForUpload(IDatabaseConnectionService databaseConnection)
        {
            using (
                DbDataReader reader =
                    databaseConnection.ExecuteReader(Constants.StatsDownloadDatabase.GetDownloadsReadyForUploadSql))
            {
                var downloadsReadyForUpload = new List<int>();

                while (reader.Read())
                {
                    downloadsReadyForUpload.Add(reader.GetInt32(0));
                }

                return downloadsReadyForUpload;
            }
        }

        private string GetFileData(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            DbParameter download = CreateDownloadIdParameter(databaseConnection, downloadId);

            DbParameter fileName = databaseConnection.CreateParameter("@FileName", DbType.String,
                ParameterDirection.Output, -1);

            DbParameter fileExtension = databaseConnection.CreateParameter("@FileExtension", DbType.String,
                ParameterDirection.Output, -1);

            DbParameter fileData = databaseConnection.CreateParameter("@FileData", DbType.String,
                ParameterDirection.Output, -1);

            databaseConnection.ExecuteStoredProcedure(Constants.StatsDownloadDatabase.GetFileDataProcedureName,
                new List<DbParameter> { download, fileName, fileExtension, fileData });

            return (string) fileData.Value;
        }

        private DateTime GetLastFileDownloadDateTime(IDatabaseConnectionService databaseConnection)
        {
            return
                databaseConnection.ExecuteScalar(Constants.StatsDownloadDatabase.GetLastFileDownloadDateTimeSql) as
                    DateTime? ?? default(DateTime);
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogMethodInvoked([CallerMemberName] string method = "")
        {
            LogVerbose($"{method} Invoked");
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private int NewFileDownloadStarted(IDatabaseConnectionService databaseConnection)
        {
            DbParameter downloadId = databaseConnection.CreateParameter("@DownloadId", DbType.Int32,
                ParameterDirection.Output);

            databaseConnection.ExecuteStoredProcedure(
                Constants.StatsDownloadDatabase.NewFileDownloadStartedProcedureName,
                new List<DbParameter> { downloadId });

            return (int) downloadId.Value;
        }

        private void SetAddUserDataParameterValues(int downloadId, UserData userData, AddUserDataParameters parameters)
        {
            parameters.DownloadId.Value = downloadId;
            parameters.FahUserName.Value = userData.Name;
            parameters.TotalPoints.Value = userData.TotalPoints;
            parameters.WorkUnits.Value = userData.TotalWorkUnits;
            parameters.TeamNumber.Value = userData.TeamNumber;
            parameters.FriendlyName.Value = userData.FriendlyName ?? DBNull.Value as object;
            parameters.BitcoinAddress.Value = userData.BitcoinAddress ?? DBNull.Value as object;
        }

        private void SetAddUserRejectionParameters(int downloadId, FailedUserData failedUserData,
            AddUserRejectionParameters parameters)
        {
            parameters.DownloadId.Value = downloadId;
            parameters.LineNumber.Value = failedUserData.LineNumber;
            parameters.RejectionReason.Value = errorMessageService.GetErrorMessage(failedUserData);
        }

        private void StartStatsUpload(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            DbParameter download = CreateDownloadIdParameter(databaseConnection, downloadId);

            databaseConnection.ExecuteStoredProcedure(Constants.StatsDownloadDatabase.StartStatsUploadProcedureName,
                new List<DbParameter> { download });
        }

        private void StatsUploadError(IDatabaseConnectionService databaseConnection,
            StatsUploadResult statsUploadResult)
        {
            DbParameter downloadId = CreateDownloadIdParameter(databaseConnection, statsUploadResult.DownloadId);
            DbParameter errorMessage = CreateErrorMessageParameter(databaseConnection, statsUploadResult);

            ExecuteStoredProcedure(databaseConnection, Constants.StatsDownloadDatabase.StatsUploadErrorProcedureName,
                new List<DbParameter> { downloadId, errorMessage });
        }

        private void StatsUploadFinished(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            DbParameter download = CreateDownloadIdParameter(databaseConnection, downloadId);

            databaseConnection.ExecuteStoredProcedure(Constants.StatsDownloadDatabase.StatsUploadFinishedProcedureName,
                new List<DbParameter> { download });
        }

        private void UpdateToLatest(IDatabaseConnectionService databaseConnection)
        {
            int numberOfRowsEffected =
                databaseConnection.ExecuteStoredProcedure(
                    Constants.StatsDownloadDatabase.UpdateToLatestStoredProcedureName);

            LogVerbose($"'{numberOfRowsEffected}' rows were effected");
        }

        private class AddUserDataParameters
        {
            public DbParameter[] AllParameters
                => new[] { DownloadId, FahUserName, TotalPoints, WorkUnits, TeamNumber, FriendlyName, BitcoinAddress };

            public DbParameter BitcoinAddress { get; set; }

            public DbParameter DownloadId { get; set; }

            public DbParameter FahUserName { get; set; }

            public DbParameter FriendlyName { get; set; }

            public DbParameter TeamNumber { get; set; }

            public DbParameter TotalPoints { get; set; }

            public DbParameter WorkUnits { get; set; }
        }

        private class AddUserRejectionParameters
        {
            public DbParameter[] AllParameters => new[] { DownloadId, LineNumber, RejectionReason };

            public DbParameter DownloadId { get; set; }

            public DbParameter LineNumber { get; set; }

            public DbParameter RejectionReason { get; set; }
        }
    }
}