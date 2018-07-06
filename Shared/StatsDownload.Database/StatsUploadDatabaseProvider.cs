namespace StatsDownload.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;
    using Core.Interfaces.Logging;

    public class StatsUploadDatabaseProvider : IStatsUploadDatabaseService
    {
        private const string DatabaseConnectionSuccessfulLogMessage = "Database connection was successful";

        private readonly IDatabaseConnectionServiceFactory databaseConnectionServiceFactory;

        private readonly IDatabaseConnectionSettingsService databaseConnectionSettingsService;

        private readonly IErrorMessageService errorMessageService;

        private readonly ILoggingService loggingService;

        public StatsUploadDatabaseProvider(IDatabaseConnectionSettingsService databaseConnectionSettingsService,
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

        public void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> users,
            IList<FailedUserData> failedUsers)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service =>
            {
                AddUsers(transaction, service, downloadId, users, failedUsers);
            });
        }

        public void Commit(DbTransaction transaction)
        {
            transaction?.Commit();
        }

        public DbTransaction CreateTransaction()
        {
            LogMethodInvoked();
            DbTransaction transaction = null;
            CreateDatabaseConnectionAndExecuteAction(service => { transaction = CreateTransaction(service); });
            return transaction;
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

        public void Rollback(DbTransaction transaction)
        {
            LogMethodInvoked();
            transaction?.Rollback();
        }

        public void StartStatsUpload(DbTransaction transaction, int downloadId, DateTime downloadDateTime)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service =>
                StartStatsUpload(service, transaction, downloadId, downloadDateTime));
        }

        public void StatsUploadError(StatsUploadResult statsUploadResult)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => StatsUploadError(service, statsUploadResult));
        }

        public void StatsUploadFinished(DbTransaction transaction, int downloadId)
        {
            LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service =>
                StatsUploadFinished(transaction, service, downloadId));
        }

        private bool AddUserCommandFailed(AddUserDataParameters addUserParameters)
        {
            return !AddUserCommandSuccess(addUserParameters);
        }

        private bool AddUserCommandSuccess(AddUserDataParameters addUserParameters)
        {
            return addUserParameters.ReturnValue.Value is int returnValue && returnValue == 0;
        }

        private void AddUserRejections(IDatabaseConnectionService databaseConnection, DbTransaction transaction,
            int downloadId,
            IEnumerable<FailedUserData> failedUsersData)
        {
            AddUserRejectionParameters parameters = CreateAddUserRejectionParameters(databaseConnection);

            using (DbCommand command = databaseConnection.CreateDbCommand())
            {
                command.CommandText = Constants.StatsDownloadDatabase.AddUserRejectionProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                command.Parameters.AddRange(parameters.AllParameters);

                foreach (FailedUserData failedUserData in failedUsersData ?? new FailedUserData[0])
                {
                    SetAddUserRejectionParameters(downloadId, failedUserData, parameters);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddUsers(DbTransaction transaction, IDatabaseConnectionService databaseConnection, int downloadId,
            IEnumerable<UserData> users, IList<FailedUserData> failedUsers)
        {
            AddUserRejections(databaseConnection, transaction, downloadId, failedUsers);
            AddUsersData(databaseConnection, transaction, downloadId, users, failedUsers);
        }

        private void AddUsersData(IDatabaseConnectionService databaseConnection, DbTransaction transaction,
            int downloadId,
            IEnumerable<UserData> usersData,
            IList<FailedUserData> failedUsers)
        {
            AddUserDataParameters addUserParameters = CreateAddUserDataParameters(databaseConnection);

            using (DbCommand addUserDataCommand = databaseConnection.CreateDbCommand())
            {
                using (DbCommand rebuildIndicesCommand = databaseConnection.CreateDbCommand())
                {
                    addUserDataCommand.CommandText = Constants.StatsDownloadDatabase.AddUserDataProcedureName;
                    addUserDataCommand.CommandType = CommandType.StoredProcedure;
                    addUserDataCommand.Transaction = transaction;
                    addUserDataCommand.Parameters.AddRange(addUserParameters.AllParameters);

                    rebuildIndicesCommand.CommandText = Constants.StatsDownloadDatabase.RebuildIndicesProcedureName;
                    rebuildIndicesCommand.CommandType = CommandType.StoredProcedure;
                    rebuildIndicesCommand.Transaction = transaction;

                    for (var index = 0; index < (usersData?.Count() ?? 0); index++)
                    {
                        UserData userData = usersData.ElementAt(index);
                        SetAddUserDataParameterValues(downloadId, userData, addUserParameters);
                        addUserDataCommand.ExecuteNonQuery();

                        if (AddUserCommandFailed(addUserParameters))
                        {
                            failedUsers.Add(new FailedUserData(userData.LineNumber, RejectionReason.FailedAddToDatabase,
                                userData));
                        }

                        if (index % 2500 == 0)
                        {
                            rebuildIndicesCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private AddUserDataParameters CreateAddUserDataParameters(IDatabaseConnectionService databaseConnection)
        {
            return new AddUserDataParameters
            {
                DownloadId = CreateDownloadIdParameter(databaseConnection),
                LineNumber = CreateLineNumberParameter(databaseConnection),
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
                        ParameterDirection.Input),
                ReturnValue =
                    databaseConnection.CreateParameter("@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue)
            };
        }

        private AddUserRejectionParameters CreateAddUserRejectionParameters(
            IDatabaseConnectionService databaseConnection)
        {
            return new AddUserRejectionParameters
            {
                DownloadId = CreateDownloadIdParameter(databaseConnection),
                LineNumber = CreateLineNumberParameter(databaseConnection),
                RejectionReason =
                    databaseConnection.CreateParameter("@RejectionReason",
                        DbType.String, ParameterDirection.Input)
            };
        }

        private IDatabaseConnectionService CreateDatabaseConnection(string connectionString, int? commandTimeout)
        {
            return databaseConnectionServiceFactory.Create(connectionString, commandTimeout);
        }

        private void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            string connectionString = GetConnectionString();
            int? commandTimeout = GetCommandTimeout();
            EnsureValidConnectionString(connectionString);
            IDatabaseConnectionService databaseConnection = CreateDatabaseConnection(connectionString, commandTimeout);
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
            string message =
                errorMessageService.GetErrorMessage(fileDownloadResult.FailedReason, StatsDownloadService.StatsUpload);
            return CreateErrorMessageParameter(databaseConnection, message);
        }

        private DbParameter CreateLineNumberParameter(IDatabaseConnectionService databaseConnection)
        {
            return databaseConnection.CreateParameter("@LineNumber", DbType.Int32,
                ParameterDirection.Input);
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

        private void ExecuteStoredProcedure(IDatabaseConnectionService databaseConnection, string storedProcedure,
            List<DbParameter> parameters)
        {
            databaseConnection.ExecuteStoredProcedure(storedProcedure, parameters);
        }

        private int? GetCommandTimeout()
        {
            return databaseConnectionSettingsService.GetCommandTimeout();
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

        private void SetAddUserDataParameterValues(int downloadId, UserData userData, AddUserDataParameters parameters)
        {
            parameters.DownloadId.Value = downloadId;
            parameters.LineNumber.Value = userData.LineNumber;
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

        private void StartStatsUpload(IDatabaseConnectionService databaseConnection, DbTransaction transaction,
            int downloadId,
            DateTime downloadDateTime)
        {
            DbParameter download = CreateDownloadIdParameter(databaseConnection, downloadId);

            DbParameter downloadDateTimeParameter =
                databaseConnection.CreateParameter("@DownloadDateTime", DbType.DateTime, ParameterDirection.Input);
            downloadDateTimeParameter.Value = downloadDateTime;

            databaseConnection.ExecuteStoredProcedure(transaction,
                Constants.StatsDownloadDatabase.StartStatsUploadProcedureName,
                new List<DbParameter> { download, downloadDateTimeParameter });
        }

        private void StatsUploadError(IDatabaseConnectionService databaseConnection,
            StatsUploadResult statsUploadResult)
        {
            DbParameter downloadId = CreateDownloadIdParameter(databaseConnection, statsUploadResult.DownloadId);
            DbParameter errorMessage = CreateErrorMessageParameter(databaseConnection, statsUploadResult);

            ExecuteStoredProcedure(databaseConnection, Constants.StatsDownloadDatabase.StatsUploadErrorProcedureName,
                new List<DbParameter> { downloadId, errorMessage });
        }

        private void StatsUploadFinished(DbTransaction transaction, IDatabaseConnectionService databaseConnection,
            int downloadId)
        {
            DbParameter download = CreateDownloadIdParameter(databaseConnection, downloadId);

            databaseConnection.ExecuteStoredProcedure(transaction,
                Constants.StatsDownloadDatabase.StatsUploadFinishedProcedureName,
                new List<DbParameter> { download });
        }

        private class AddUserDataParameters
        {
            public DbParameter[] AllParameters
                => new[]
                {
                    DownloadId,
                    LineNumber,
                    FahUserName,
                    TotalPoints,
                    WorkUnits,
                    TeamNumber,
                    FriendlyName,
                    BitcoinAddress,
                    ReturnValue
                };

            public DbParameter BitcoinAddress { get; set; }

            public DbParameter DownloadId { get; set; }

            public DbParameter FahUserName { get; set; }

            public DbParameter FriendlyName { get; set; }

            public DbParameter LineNumber { get; set; }

            public DbParameter ReturnValue { get; set; }

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