namespace StatsDownload.Database.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    using NSubstitute;
    using NSubstitute.ClearExtensions;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;

    [TestFixture]
    public class TestStatsUploadDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();

            statsDownloadDatabaseServiceMock = Substitute.For<IStatsDownloadDatabaseService>();
            statsDownloadDatabaseServiceMock.When(service =>
                service.CreateDatabaseConnectionAndExecuteAction(
                    Arg.Any<Action<IDatabaseConnectionService>>())).Do(callInfo =>
            {
                var service = callInfo.Arg<Action<IDatabaseConnectionService>>();

                service.Invoke(databaseConnectionServiceMock);
            });

            statsDownloadDatabaseParameterServiceMock = Substitute.For<IStatsDownloadDatabaseParameterService>();

            loggingServiceMock = Substitute.For<ILoggingService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = NewStatsUploadDatabaseProvider(statsDownloadDatabaseServiceMock,
                statsDownloadDatabaseParameterServiceMock, loggingServiceMock, errorMessageServiceMock);

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

            dbDataReaderMock = Substitute.For<DbDataReader>();
            dbDataReaderMock.Read().Returns(true, true, true, false);
            dbDataReaderMock.GetInt32(0).Returns(100, 200, 300);

            databaseConnectionServiceMock
                .ExecuteReader("SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]")
                .Returns(dbDataReaderMock);

            downloadIdParameterMock = Substitute.For<DbParameter>();
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock, 100)
                                                     .Returns(downloadIdParameterMock);
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock, 200)
                                                     .Returns(downloadIdParameterMock);
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock, 300)
                                                     .Returns(downloadIdParameterMock);
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock)
                                                     .Returns(downloadIdParameterMock);

            errorMessageParameterMock = Substitute.For<DbParameter>();
            rejectionReasonParameterMock = Substitute.For<DbParameter>();
            statsDownloadDatabaseParameterServiceMock.CreateRejectionReasonParameter(databaseConnectionServiceMock)
                                                     .Returns(rejectionReasonParameterMock);
        }

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private DbDataReader dbDataReaderMock;

        private DbParameter downloadIdParameterMock;

        private DbParameter errorMessageParameterMock;

        private IErrorMessageService errorMessageServiceMock;

        private ILoggingService loggingServiceMock;

        private DbParameter rejectionReasonParameterMock;

        private IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterServiceMock;

        private IStatsDownloadDatabaseService statsDownloadDatabaseServiceMock;

        private IStatsUploadDatabaseService systemUnderTest;

        [Test]
        public void AddUsers_WhenAddUserDataFails_AddsFailedUserToFailedList()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns(1);

            databaseConnectionServiceMock.ClearSubstitute();
            var command = Substitute.For<DbCommand>();
            command.Parameters.Returns(Substitute.For<DbParameterCollection>());
            databaseConnectionServiceMock.CreateDbCommand().Returns(command);

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock,
                ParameterDirection.Input);

            databaseConnectionServiceMock.CreateParameter("@ReturnValue", DbType.Int32, ParameterDirection.ReturnValue)
                                         .Returns(dbParameter);

            var user1 = new UserData(999, "name", 10, 100, 1000)
                        {
                            BitcoinAddress = "address", FriendlyName = "friendly"
                        };
            var user2 = new UserData(900, "name", 10, 100, 1000)
                        {
                            BitcoinAddress = "address", FriendlyName = "friendly"
                        };
            IList<FailedUserData> failedUsers = new List<FailedUserData>();

            systemUnderTest.AddUsers(null, 100, new List<UserData> { user1, user2 }, failedUsers);

            Assert.That(failedUsers.Count, Is.EqualTo(2));

            Assert.That(failedUsers[0].RejectionReason, Is.EqualTo(RejectionReason.FailedAddToDatabase));
            Assert.That(failedUsers[0].UserData, Is.EqualTo(user1));
            Assert.That(failedUsers[0].LineNumber, Is.EqualTo(999));

            Assert.That(failedUsers[1].RejectionReason, Is.EqualTo(RejectionReason.FailedAddToDatabase));
            Assert.That(failedUsers[1].UserData, Is.EqualTo(user2));
            Assert.That(failedUsers[1].LineNumber, Is.EqualTo(900));
        }

        [Test]
        public void AddUsers_WhenBitcoinAddressExceedsSize_UserIsRejected()
        {
            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { null, parameters => { actualParameters = parameters; } });

            errorMessageServiceMock.GetErrorMessage(Arg.Is<FailedUserData>(failedUser =>
                                       failedUser.RejectionReason == RejectionReason.BitcoinAddressExceedsMaxSize))
                                   .Returns("btc");

            InvokeAddUsers(users: new List<UserData>
                                  {
                                      new UserData(0, "", 10, 100, 1000) { BitcoinAddress = new string(' ', 51) }
                                  });

            Assert.That(actualParameters.Count, Is.AtLeast(2));
            Assert.That(actualParameters[2], Is.EqualTo(rejectionReasonParameterMock));
            Assert.That(actualParameters[2].Value, Is.EqualTo("btc"));
        }

        [Test]
        public void AddUsers_WhenFahNameExceedsSize_UserIsRejected()
        {
            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { null, parameters => { actualParameters = parameters; } });

            errorMessageServiceMock
                .GetErrorMessage(Arg.Is<FailedUserData>(failedUser =>
                    failedUser.RejectionReason == RejectionReason.FahNameExceedsMaxSize)).Returns("name");

            InvokeAddUsers(users: new List<UserData> { new UserData(0, new string(' ', 151), 10, 100, 1000) });

            Assert.That(actualParameters.Count, Is.AtLeast(2));
            Assert.That(actualParameters[2], Is.EqualTo(rejectionReasonParameterMock));
            Assert.That(actualParameters[2].Value, Is.EqualTo("name"));
        }

        [Test]
        public void AddUsers_WhenFailedUserListNull_ThrowsNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                InvokeAddUsers(null, Substitute.For<DbTransaction>(), 0, new UserData[0]));
        }

        [Test]
        public void AddUsers_WhenFriendlyNameExceedsSize_UserIsRejected()
        {
            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { null, parameters => { actualParameters = parameters; } });

            errorMessageServiceMock.GetErrorMessage(Arg.Is<FailedUserData>(failedUser =>
                                       failedUser.RejectionReason == RejectionReason.FriendlyNameExceedsMaxSize))
                                   .Returns("friendly");

            InvokeAddUsers(users: new List<UserData>
                                  {
                                      new UserData(0, "", 10, 100, 1000) { FriendlyName = new string(' ', 126) }
                                  });

            Assert.That(actualParameters.Count, Is.AtLeast(2));
            Assert.That(actualParameters[2], Is.EqualTo(rejectionReasonParameterMock));
            Assert.That(actualParameters[2].Value, Is.EqualTo("friendly"));
        }

        [Test]
        public void AddUsers_WhenInvoked_AddsUsers()
        {
            DbCommand failedUsersCommand = null;
            DbCommand addUsersCommand = null;
            DbCommand rebuildIndicesCommand = null;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[]
                                                       {
                                                           dbCommand => addUsersCommand = dbCommand,
                                                           dbCommand => rebuildIndicesCommand = dbCommand,
                                                           dbCommand => failedUsersCommand = dbCommand
                                                       });

            InvokeAddUsers(users: new List<UserData> { new UserData(), new UserData(), new UserData() },
                failedUsers: new List<FailedUserData> { new FailedUserData(), new FailedUserData() });

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.AddUsers));
                addUsersCommand.ExecuteNonQuery();
                rebuildIndicesCommand.ExecuteNonQuery();
                addUsersCommand.ExecuteNonQuery();
                addUsersCommand.ExecuteNonQuery();
                failedUsersCommand.ExecuteNonQuery();
                failedUsersCommand.ExecuteNonQuery();
            });
        }

        [Test]
        public void AddUsers_WhenInvoked_AddUserDataParametersAreProvided()
        {
            List<DbParameter> actualParameters = default;

            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { parameters => { actualParameters = parameters; } });

            systemUnderTest.AddUsers(null, 100,
                new List<UserData>
                {
                    new UserData(999, "name", 10, 100, 1000) { BitcoinAddress = "address", FriendlyName = "friendly" }
                }, new List<FailedUserData>());

            Assert.That(actualParameters.Count, Is.EqualTo(9));

            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));

            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@LineNumber"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo(999));

            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FAHUserName"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[2].Value, Is.EqualTo("name"));
            Assert.That(actualParameters[2].Size, Is.EqualTo(150));

            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@TotalPoints"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[3].Value, Is.EqualTo(10));

            Assert.That(actualParameters[4].ParameterName, Is.EqualTo("@WorkUnits"));
            Assert.That(actualParameters[4].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[4].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[4].Value, Is.EqualTo(100));

            Assert.That(actualParameters[5].ParameterName, Is.EqualTo("@TeamNumber"));
            Assert.That(actualParameters[5].DbType, Is.EqualTo(DbType.Int64));
            Assert.That(actualParameters[5].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[5].Value, Is.EqualTo(1000));

            Assert.That(actualParameters[6].ParameterName, Is.EqualTo("@FriendlyName"));
            Assert.That(actualParameters[6].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[6].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[6].Value, Is.EqualTo("friendly"));
            Assert.That(actualParameters[6].Size, Is.EqualTo(125));

            Assert.That(actualParameters[7].ParameterName, Is.EqualTo("@BitcoinAddress"));
            Assert.That(actualParameters[7].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[7].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[7].Value, Is.EqualTo("address"));
            Assert.That(actualParameters[7].Size, Is.EqualTo(50));

            Assert.That(actualParameters[8].ParameterName, Is.EqualTo("@ReturnValue"));
            Assert.That(actualParameters[8].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[8].Direction, Is.EqualTo(ParameterDirection.ReturnValue));
            Assert.That(actualParameters[8].Value, Is.EqualTo(0));
        }

        [Test]
        public void AddUsers_WhenInvoked_AddUserRejectionParametersAreProvided()
        {
            var failedUserData = new FailedUserData(10, "", RejectionReason.UnexpectedFormat, new UserData());
            errorMessageServiceMock.GetErrorMessage(failedUserData).Returns("RejectionReason");

            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { null, parameters => { actualParameters = parameters; } });

            systemUnderTest.AddUsers(null, 100, null, new[] { failedUserData });

            Assert.That(actualParameters.Count, Is.EqualTo(3));

            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));

            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@LineNumber"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo(10));

            Assert.That(actualParameters[2], Is.EqualTo(rejectionReasonParameterMock));
            Assert.That(actualParameters[2].Value, Is.EqualTo("RejectionReason"));
        }

        [Test]
        public void AddUsers_WhenInvoked_DisposesCommands()
        {
            DbCommand failedUsersCommand = null;
            DbCommand addUsersCommand = null;
            DbCommand rebuildIndicesCommand = null;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[]
                                                       {
                                                           dbCommand => failedUsersCommand = dbCommand,
                                                           dbCommand => addUsersCommand = dbCommand,
                                                           dbCommand => rebuildIndicesCommand = dbCommand
                                                       });

            systemUnderTest.AddUsers(null, 1, new[] { new UserData(), new UserData() }, new List<FailedUserData>());

            failedUsersCommand.Received(1).Dispose();
            addUsersCommand.Received(1).Dispose();
            rebuildIndicesCommand.Received(1).Dispose();
        }

        [Test]
        public void AddUsers_WhenInvoked_RebuildsIndicesPeriodically()
        {
            DbCommand command = null;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[]
                                                       {
                                                           null, dbCommand => command = dbCommand, null
                                                       });

            var users = new UserData[2501];
            for (var index = 0; index < users.Length; index++)
            {
                users[index] = new UserData();
            }

            InvokeAddUsers(users: users);

            command.Received(2).ExecuteNonQuery();
        }

        [Test]
        public void AddUsers_WhenInvoked_ReusesCommands()
        {
            SetUpDatabaseConnectionCreateDbCommandMock();

            InvokeAddUsers();

            databaseConnectionServiceMock.Received(3).CreateDbCommand();
        }

        [Test]
        public void AddUsers_WhenInvoked_ReusesParameters()
        {
            SetUpDatabaseConnectionCreateDbCommandMock();

            InvokeAddUsers();

            databaseConnectionServiceMock.ReceivedWithAnyArgs(6)
                                         .CreateParameter(null, DbType.AnsiString, ParameterDirection.Input);
            databaseConnectionServiceMock.ReceivedWithAnyArgs(3)
                                         .CreateParameter(null, DbType.AnsiString, ParameterDirection.Input, 0);
        }

        [Test]
        public void AddUsers_WhenInvoked_UsesAddUserDataStoredProcedure()
        {
            DbCommand command = default;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[] { dbCommand => command = dbCommand });

            var transactionMock = Substitute.For<DbTransaction>();

            InvokeAddUsers(transactionMock);

            command.Received(1).CommandText = "[FoldingCoin].[AddUserData]";
            command.Received(1).CommandType = CommandType.StoredProcedure;
            command.Received(1).Transaction = transactionMock;
        }

        [Test]
        public void AddUsers_WhenInvoked_UsesAddUserRejectionStoredProcedure()
        {
            DbCommand command = default;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[]
                                                       {
                                                           null, null, dbCommand => command = dbCommand
                                                       });

            var transactionMock = Substitute.For<DbTransaction>();

            InvokeAddUsers(transactionMock);

            command.Received(1).CommandText = "[FoldingCoin].[AddUserRejection]";
            command.Received(1).CommandType = CommandType.StoredProcedure;
            command.Received(1).Transaction = transactionMock;
        }

        [Test]
        public void AddUsers_WhenInvoked_UsesRebuildsIndicesStoredProcedure()
        {
            DbCommand command = default;
            SetUpDatabaseConnectionCreateDbCommandMock(new Action<DbCommand>[]
                                                       {
                                                           null, dbCommand => command = dbCommand, null
                                                       });

            var transactionMock = Substitute.For<DbTransaction>();

            InvokeAddUsers(transactionMock);

            command.Received(1).CommandText = "[FoldingCoin].[RebuildIndices]";
            command.Received(1).CommandType = CommandType.StoredProcedure;
            command.Received(1).Transaction = transactionMock;
        }

        [Test]
        public void AddUsers_WhenNullBitcoinAddress_ParameterIsDBNull()
        {
            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { parameters => { actualParameters = parameters; } });

            InvokeAddUsers(users: new List<UserData>
                                  {
                                      new UserData(0, "name", 10, 100, 1000) { FriendlyName = "friendly" }
                                  });

            Assert.That(actualParameters.Count, Is.EqualTo(9));
            Assert.That(actualParameters[7].ParameterName, Is.EqualTo("@BitcoinAddress"));
            Assert.That(actualParameters[7].Value, Is.EqualTo(DBNull.Value));
        }

        [Test]
        public void AddUsers_WhenNullFriendlyName_ParameterIsDBNull()
        {
            List<DbParameter> actualParameters = default;
            SetUpDatabaseConnectionCreateDbCommandMock(null,
                new Action<List<DbParameter>>[] { parameters => { actualParameters = parameters; } });

            InvokeAddUsers(users: new List<UserData>
                                  {
                                      new UserData(0, "name", 10, 100, 1000) { BitcoinAddress = "address" }
                                  });

            Assert.That(actualParameters.Count, Is.AtLeast(9));
            Assert.That(actualParameters[6].ParameterName, Is.EqualTo("@FriendlyName"));
            Assert.That(actualParameters[6].Value, Is.EqualTo(DBNull.Value));
        }

        [Test]
        public void Commit_WhenInvoked_CommitsTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Commit(transaction);

            statsDownloadDatabaseServiceMock.Received().Commit(transaction);
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadDatabaseProvider(null,
                statsDownloadDatabaseParameterServiceMock, loggingServiceMock, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadDatabaseProvider(statsDownloadDatabaseServiceMock,
                null, loggingServiceMock, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadDatabaseProvider(statsDownloadDatabaseServiceMock,
                statsDownloadDatabaseParameterServiceMock, null, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadDatabaseProvider(statsDownloadDatabaseServiceMock,
                statsDownloadDatabaseParameterServiceMock, loggingServiceMock, null));
        }

        [Test]
        public void CreateTransaction_WhenInvoked_ReturnsTransaction()
        {
            var expected = Substitute.For<DbTransaction>();
            statsDownloadDatabaseServiceMock.CreateTransaction().Returns(expected);

            DbTransaction actual = systemUnderTest.CreateTransaction();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenInvoked_DisposesReader()
        {
            systemUnderTest.GetDownloadsReadyForUpload();

            dbDataReaderMock.Received().Dispose();
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenInvoked_GetsDownloadsReadyForUpload()
        {
            systemUnderTest.GetDownloadsReadyForUpload();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.GetDownloadsReadyForUpload));
                databaseConnectionServiceMock.ExecuteReader(
                    "SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]");
            });
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenInvoked_ReturnsDownloadIds()
        {
            List<int> actual = systemUnderTest.GetDownloadsReadyForUpload().ToList();

            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual[1], Is.EqualTo(200));
        }

        [Test]
        public void GetDownloadsReadyForUpload_WhenNoFilesReadyForUpload_ReturnsEmptyList()
        {
            dbDataReaderMock.ClearSubstitute();
            dbDataReaderMock.Read().Returns(false);

            List<int> actual = systemUnderTest.GetDownloadsReadyForUpload().ToList();

            Assert.That(actual.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetFileData_WhenFileDataDbNull_ReturnsNull()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns(DBNull.Value);

            databaseConnectionServiceMock.ClearSubstitute();
            databaseConnectionServiceMock.CreateParameter("@FileData", DbType.String, ParameterDirection.Output, -1)
                                         .Returns(dbParameter);
            databaseConnectionServiceMock.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Input)
                                         .Returns(Substitute.For<DbParameter>());

            string actual = systemUnderTest.GetFileData(100);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void GetFileData_WhenInvoked_GetFileData()
        {
            systemUnderTest.GetFileData(100);

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.GetFileData));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[GetFileData]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void GetFileData_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default;

            databaseConnectionServiceMock
                .When(service =>
                    service.ExecuteStoredProcedure("[FoldingCoin].[GetFileData]", Arg.Any<List<DbParameter>>()))
                .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            systemUnderTest.GetFileData(100);

            Assert.That(actualParameters.Count, Is.EqualTo(4));

            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));

            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FileName"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[1].Size, Is.EqualTo(-1));

            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FileExtension"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[2].Size, Is.EqualTo(-1));

            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@FileData"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
            Assert.That(actualParameters[3].Size, Is.EqualTo(-1));
        }

        [Test]
        public void GetFileData_WhenInvoked_ReturnsFileData()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns("FileData");

            databaseConnectionServiceMock.ClearSubstitute();
            databaseConnectionServiceMock.CreateParameter("@FileData", DbType.String, ParameterDirection.Output, -1)
                                         .Returns(dbParameter);
            databaseConnectionServiceMock.CreateParameter("@DownloadId", DbType.Int32, ParameterDirection.Input)
                                         .Returns(Substitute.For<DbParameter>());

            string actual = systemUnderTest.GetFileData(100);

            Assert.That(actual, Is.EqualTo("FileData"));
        }

        [TestCase(true, DatabaseFailedReason.None, FailedReason.None)]
        [TestCase(false, DatabaseFailedReason.DatabaseUnavailable, FailedReason.DatabaseUnavailable)]
        public void IsAvailable_WhenInvoked_ReturnsDatabaseAvailability(bool expectedIsAvailable,
                                                                        DatabaseFailedReason failedReason,
                                                                        FailedReason expectedReason)
        {
            statsDownloadDatabaseServiceMock.IsAvailable(Constants.StatsUploadDatabase.StatsUploadObjects)
                                            .Returns((expectedIsAvailable, failedReason));

            (bool isAvailable, FailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.EqualTo(expectedIsAvailable));
            Assert.That(actual.reason, Is.EqualTo(expectedReason));
        }

        [Test]
        public void Rollback_WhenInvoked_RollsBackTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Rollback(transaction);

            statsDownloadDatabaseServiceMock.Received().Rollback(transaction);
        }

        [Test]
        public void StartStatsUpload_WhenInvoked_ParameterIsProvided()
        {
            var transaction = Substitute.For<DbTransaction>();

            List<DbParameter> actualParameters = default;

            databaseConnectionServiceMock
                .When(service => service.ExecuteStoredProcedure(transaction, "[FoldingCoin].[StartStatsUpload]",
                    Arg.Any<List<DbParameter>>())).Do(callback =>
                {
                    actualParameters = callback.Arg<List<DbParameter>>();
                });

            DateTime dateTime = DateTime.UtcNow;

            systemUnderTest.StartStatsUpload(transaction, 100, dateTime);

            Assert.That(actualParameters.Count, Is.EqualTo(2));

            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));

            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@DownloadDateTime"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.DateTime));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo(dateTime));
        }

        [Test]
        public void StartStatsUpload_WhenInvoked_StartsStatsUpload()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.StartStatsUpload(transaction, 1, DateTime.UtcNow);

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.StartStatsUpload));
                databaseConnectionServiceMock.ExecuteStoredProcedure(transaction, "[FoldingCoin].[StartStatsUpload]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void StatsUploadError_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default;

            databaseConnectionServiceMock
                .When(service =>
                    service.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadError]", Arg.Any<List<DbParameter>>()))
                .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            var uploadResult = new StatsUploadResult(100, FailedReason.UnexpectedException);

            statsDownloadDatabaseParameterServiceMock
                .CreateErrorMessageParameter(databaseConnectionServiceMock, uploadResult)
                .Returns(errorMessageParameterMock);

            systemUnderTest.StatsUploadError(uploadResult);

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            Assert.That(actualParameters[1], Is.EqualTo(errorMessageParameterMock));
        }

        [Test]
        public void StatsUploadError_WhenInvoked_UpdatesStatsUploadToError()
        {
            systemUnderTest.StatsUploadError(new StatsUploadResult());

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.StatsUploadError));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[StatsUploadError]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void StatsUploadFinished_WhenInvoked_ParametersAreProvided()
        {
            var transaction = Substitute.For<DbTransaction>();

            List<DbParameter> actualParameters = default;

            databaseConnectionServiceMock
                .When(service => service.ExecuteStoredProcedure(transaction, "[FoldingCoin].[StatsUploadFinished]",
                    Arg.Any<List<DbParameter>>())).Do(callback =>
                {
                    actualParameters = callback.Arg<List<DbParameter>>();
                });

            systemUnderTest.StatsUploadFinished(transaction, 100);

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
        }

        [Test]
        public void StatsUploadFinished_WhenInvoked_UpdatesStatsUploadToFinished()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.StatsUploadFinished(transaction, 100);

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.StatsUploadFinished));
                databaseConnectionServiceMock.ExecuteStoredProcedure(transaction, "[FoldingCoin].[StatsUploadFinished]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        private void InvokeAddUsers(DbTransaction transaction = null, int downloadId = 100,
                                    IEnumerable<UserData> users = null)
        {
            InvokeAddUsers(new List<FailedUserData>(), transaction, downloadId, users);
        }

        private void InvokeAddUsers(IList<FailedUserData> failedUsers, DbTransaction transaction = null,
                                    int downloadId = 100, IEnumerable<UserData> users = null)
        {
            systemUnderTest.AddUsers(transaction, downloadId, users, failedUsers);
        }

        private (bool isAvailable, FailedReason reason) InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private IStatsUploadDatabaseService NewStatsUploadDatabaseProvider(
            IStatsDownloadDatabaseService statsDownloadDatabaseService,
            IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterService,
            ILoggingService loggingService, IErrorMessageService errorMessageService)
        {
            return new StatsUploadDatabaseProvider(statsDownloadDatabaseService, statsDownloadDatabaseParameterService,
                loggingService, errorMessageService);
        }

        private void SetUpDatabaseConnectionCreateDbCommandMock(
            Action<DbCommand>[] additionalCreateDbCommandSetupActions = null,
            Action<List<DbParameter>>[] additionalAddRangeSetUpActions = null)
        {
            var createCommandCallCount = 0;
            var addRangeCallCount = 0;

            databaseConnectionServiceMock.CreateDbCommand().Returns(createDbCommandInfo =>
            {
                var command = Substitute.For<DbCommand>();
                command.Parameters.Returns(parametersInfo =>
                {
                    var parameters = Substitute.For<DbParameterCollection>();
                    parameters.When(collection => collection.AddRange(Arg.Any<Array>())).Do(addRangeInfo =>
                    {
                        if (addRangeCallCount + 1 <= additionalAddRangeSetUpActions?.Length)
                        {
                            additionalAddRangeSetUpActions[addRangeCallCount]?.Invoke(
                                addRangeInfo.Arg<Array>().Cast<DbParameter>().ToList());
                            addRangeCallCount++;
                        }
                    });
                    return parameters;
                });

                if (createCommandCallCount + 1 <= additionalCreateDbCommandSetupActions?.Length)
                {
                    additionalCreateDbCommandSetupActions[createCommandCallCount]?.Invoke(command);
                    createCommandCallCount++;
                }

                return command;
            });
        }
    }
}