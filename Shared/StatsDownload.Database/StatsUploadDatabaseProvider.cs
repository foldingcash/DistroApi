namespace StatsDownload.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;
    using Core.Interfaces.Logging;

    public class StatsUploadDatabaseProvider : IStatsUploadDatabaseService
    {
        private readonly IErrorMessageService errorMessageService;

        private readonly ILoggingService loggingService;

        private readonly IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterService;

        private readonly IStatsDownloadDatabaseService statsDownloadDatabaseService;

        public StatsUploadDatabaseProvider(IStatsDownloadDatabaseService statsDownloadDatabaseService,
            IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterService,
            ILoggingService loggingService, IErrorMessageService errorMessageService)
        {
            this.statsDownloadDatabaseService = statsDownloadDatabaseService ??
                                                throw new ArgumentNullException(nameof(statsDownloadDatabaseService));
            this.statsDownloadDatabaseParameterService = statsDownloadDatabaseParameterService ??
                                                         throw new ArgumentNullException(
                                                             nameof(statsDownloadDatabaseParameterService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.errorMessageService =
                errorMessageService ?? throw new ArgumentNullException(nameof(errorMessageService));
        }

        public void AddUsers(DbTransaction transaction, int downloadId, IEnumerable<UserData> users,
            IList<FailedUserData> failedUsers)
        {
            loggingService.LogMethodInvoked();

            if (failedUsers == null)
            {
                throw new ArgumentNullException(nameof(failedUsers));
            }

            CreateDatabaseConnectionAndExecuteAction(service =>
            {
                AddUsers(transaction, service, downloadId, users, failedUsers);
            });
        }

        public void Commit(DbTransaction transaction)
        {
            statsDownloadDatabaseService.Commit(transaction);
        }

        public DbTransaction CreateTransaction()
        {
            return statsDownloadDatabaseService.CreateTransaction();
        }

        public IEnumerable<int> GetDownloadsReadyForUpload()
        {
            loggingService.LogMethodInvoked();
            List<int> downloadsReadyForUpload = default(List<int>);
            CreateDatabaseConnectionAndExecuteAction(
                service => downloadsReadyForUpload = GetDownloadsReadyForUpload(service));
            return downloadsReadyForUpload;
        }

        public string GetFileData(int downloadId)
        {
            loggingService.LogMethodInvoked();
            string fileData = default(string);
            CreateDatabaseConnectionAndExecuteAction(service => fileData = GetFileData(service, downloadId));
            return fileData;
        }

        public (bool isAvailable, FailedReason reason) IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable();
        }

        public void Rollback(DbTransaction transaction)
        {
            statsDownloadDatabaseService.Rollback(transaction);
        }

        public void StartStatsUpload(DbTransaction transaction, int downloadId, DateTime downloadDateTime)
        {
            loggingService.LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service =>
                StartStatsUpload(service, transaction, downloadId, downloadDateTime));
        }

        public void StatsUploadError(StatsUploadResult statsUploadResult)
        {
            loggingService.LogMethodInvoked();
            CreateDatabaseConnectionAndExecuteAction(service => StatsUploadError(service, statsUploadResult));
        }

        public void StatsUploadFinished(DbTransaction transaction, int downloadId)
        {
            loggingService.LogMethodInvoked();
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
            AddUsersData(databaseConnection, transaction, downloadId, users, failedUsers);
            AddUserRejections(databaseConnection, transaction, downloadId, failedUsers);
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

                    var index = 0;

                    foreach (UserData userData in usersData ?? new UserData[0])
                    {
                        if (!IsUserDataValid(addUserParameters, userData, out RejectionReason rejectionReason))
                        {
                            failedUsers.Add(new FailedUserData(userData.LineNumber, rejectionReason,
                                userData));
                            continue;
                        }

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

                        index++;
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
                        ParameterDirection.Input, 150),
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
                        ParameterDirection.Input, 125),
                BitcoinAddress =
                    databaseConnection.CreateParameter("@BitcoinAddress", DbType.String,
                        ParameterDirection.Input, 50),
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
                    statsDownloadDatabaseParameterService.CreateRejectionReasonParameter(databaseConnection)
            };
        }

        private void CreateDatabaseConnectionAndExecuteAction(Action<IDatabaseConnectionService> action)
        {
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(action);
        }

        private DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection, int downloadId)
        {
            return statsDownloadDatabaseParameterService.CreateDownloadIdParameter(databaseConnection, downloadId);
        }

        private DbParameter CreateDownloadIdParameter(IDatabaseConnectionService databaseConnection)
        {
            return statsDownloadDatabaseParameterService.CreateDownloadIdParameter(databaseConnection);
        }

        private DbParameter CreateLineNumberParameter(IDatabaseConnectionService databaseConnection)
        {
            return databaseConnection.CreateParameter("@LineNumber", DbType.Int32,
                ParameterDirection.Input);
        }

        private void ExecuteStoredProcedure(IDatabaseConnectionService databaseConnection, string storedProcedure,
            List<DbParameter> parameters)
        {
            databaseConnection.ExecuteStoredProcedure(storedProcedure, parameters);
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

        private bool IsUserDataValid(AddUserDataParameters addUserParameters, UserData userData,
            out RejectionReason rejectionReason)
        {
            if (userData.Name?.Length > addUserParameters.FahUserName.Size)
            {
                rejectionReason = RejectionReason.FahNameExceedsMaxSize;
                return false;
            }

            if (userData.FriendlyName?.Length > addUserParameters.FriendlyName.Size)
            {
                rejectionReason = RejectionReason.FriendlyNameExceedsMaxSize;
                return false;
            }

            if (userData.BitcoinAddress?.Length > addUserParameters.BitcoinAddress.Size)
            {
                rejectionReason = RejectionReason.BitcoinAddressExceedsMaxSize;
                return false;
            }

            rejectionReason = RejectionReason.None;
            return true;
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
            DbParameter errorMessage =
                statsDownloadDatabaseParameterService.CreateErrorMessageParameter(databaseConnection,
                    statsUploadResult);

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