namespace StatsDownload.Database.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;
    using Core.Interfaces.Logging;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsDownloadDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            databaseConnectionSettingsServiceMock = Substitute.For<IDatabaseConnectionSettingsService>();
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns("connectionString");
            databaseConnectionSettingsServiceMock.GetCommandTimeout().Returns(42);

            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();
            databaseConnectionServiceFactoryMock = Substitute.For<IDatabaseConnectionServiceFactory>();
            databaseConnectionServiceFactoryMock.Create("connectionString", 42).Returns(databaseConnectionServiceMock);

            loggingServiceMock = Substitute.For<ILoggingService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                databaseConnectionServiceFactoryMock, loggingServiceMock, errorMessageServiceMock);

            TestDatabaseProviderHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

            dbDataReaderMock = Substitute.For<DbDataReader>();
            dbDataReaderMock.Read().Returns(true, true, true, false);
            dbDataReaderMock.GetInt32(0).Returns(100, 200, 300);

            databaseConnectionServiceMock
                .ExecuteReader("SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]")
                .Returns(dbDataReaderMock);
        }

        private IDatabaseConnectionServiceFactory databaseConnectionServiceFactoryMock;

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private IDatabaseConnectionSettingsService databaseConnectionSettingsServiceMock;

        private DbDataReader dbDataReaderMock;

        private IErrorMessageService errorMessageServiceMock;

        private ILoggingService loggingServiceMock;

        private IStatsDownloadDatabaseService systemUnderTest;

        [Test]
        public void Commit_WhenInvoked_CommitsTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Commit(transaction);

            transaction.Received(1).Commit();
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDataStoreProvider(null, databaseConnectionServiceFactoryMock, loggingServiceMock,
                        errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock, null, loggingServiceMock,
                        errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                        databaseConnectionServiceFactoryMock, null, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDataStoreProvider(databaseConnectionSettingsServiceMock,
                        databaseConnectionServiceFactoryMock, loggingServiceMock, null));
        }

        [TestCase(ParameterDirection.Input)]
        [TestCase(ParameterDirection.Output)]
        public void CreateDownloadIdParameter_WhenDirectionProvided_CreatesDownloadIdParameterWithDirection(
            ParameterDirection expected)
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock, expected);

            actual.Received().Direction = expected;
        }

        [Test]
        public void CreateDownloadIdParameter_WhenInvoked_CreatesInputDownloadIdParameter()
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock);

            Assert.That(actual.ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
        }

        [Test]
        public void CreateDownloadIdParameter_WhenValueProvided_CreatesInputDownloadIdParameterWithValue()
        {
            DbParameter actual = systemUnderTest.CreateDownloadIdParameter(databaseConnectionServiceMock, 100);

            Assert.That(actual.ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo(100));
        }

        [Test]
        public void
            CreateErrorMessageParameter_WhenInvokedWithFileDownloadResult_CreatesFileDownloadResultErrorMessageParameter()
        {
            var filePayload = new FilePayload();
            var fileDownloadResult =
                new FileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);

            errorMessageServiceMock
                .GetErrorMessage(fileDownloadResult.FailedReason, filePayload, StatsDownloadService.FileDownload)
                .Returns("Error Message");

            DbParameter actual =
                systemUnderTest.CreateErrorMessageParameter(databaseConnectionServiceMock, fileDownloadResult);

            Assert.That(actual.ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo("Error Message"));
        }

        [Test]
        public void
            CreateErrorMessageParameter_WhenInvokedWithStatsUploadResult_CreatesStatsUploadResultsErrorMessageParameter()
        {
            var statsUploadResult = new StatsUploadResult();

            errorMessageServiceMock.GetErrorMessage(statsUploadResult.FailedReason, StatsDownloadService.StatsUpload)
                                   .Returns("Error Message");

            DbParameter actual =
                systemUnderTest.CreateErrorMessageParameter(databaseConnectionServiceMock, statsUploadResult);

            Assert.That(actual.ParameterName, Is.EqualTo("@ErrorMessage"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actual.Value, Is.EqualTo("Error Message"));
        }

        [Test]
        public void CreateRejectionReasonParameter_WhenInvoked_CreatesRejectionReasonParameter()
        {
            DbParameter actual = systemUnderTest.CreateRejectionReasonParameter(databaseConnectionServiceMock);

            Assert.That(actual.ParameterName, Is.EqualTo("@RejectionReason"));
            Assert.That(actual.DbType, Is.EqualTo(DbType.String));
            Assert.That(actual.Direction, Is.EqualTo(ParameterDirection.Input));
        }

        [Test]
        public void CreateTransaction_WhenInvoked_LogsMethodInvoked()
        {
            systemUnderTest.CreateTransaction();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.CreateTransaction));
                databaseConnectionServiceMock.CreateTransaction();
            });
        }

        [Test]
        public void CreateTransaction_WhenInvoked_ReturnsCreatedTransaction()
        {
            var transactionMock = Substitute.For<DbTransaction>();
            databaseConnectionServiceMock.CreateTransaction().Returns(transactionMock);

            DbTransaction actual = systemUnderTest.CreateTransaction();

            Assert.That(actual, Is.EqualTo(transactionMock));
        }

        [Test]
        public void IsAvailable_WhenConnectionClosed_ConnectionOpened()
        {
            databaseConnectionServiceMock.ConnectionState.Returns(ConnectionState.Closed);

            InvokeIsAvailable();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.IsAvailable));
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogVerbose("Database connection was successful");
            });
        }

        [Test]
        public void IsAvailable_WhenConnectionOpen_ConnectionNotOpened()
        {
            databaseConnectionServiceMock.ConnectionState.Returns(ConnectionState.Open);

            InvokeIsAvailable();

            loggingServiceMock.DidNotReceive().LogVerbose("Database connection was successful");
            databaseConnectionServiceMock.DidNotReceive().Open();
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_LogsException()
        {
            var expected = new Exception();
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw(expected);

            InvokeIsAvailable();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.IsAvailable));
                databaseConnectionServiceMock.Open();
                loggingServiceMock.LogException(expected);
            });
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_ReturnsFalse()
        {
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw<Exception>();

            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.False);
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsAvailable_WhenInvalidConnectionString_ReturnsFalse(string connectionString)
        {
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns(connectionString);

            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsAvailable_WhenInvoked_ReturnsTrue()
        {
            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.True);
        }

        [Test]
        public void Rollback_WhenInvoked_RollsBackTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Rollback(transaction);

            transaction.Received(1).Rollback();
        }

        private bool InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private IStatsDownloadDatabaseService NewFileDownloadDataStoreProvider(
            IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory, ILoggingService loggingService,
            IErrorMessageService errorMessageService)
        {
            return new StatsDownloadDatabaseProvider(databaseConnectionSettingsService,
                databaseConnectionServiceFactory,
                loggingService, errorMessageService);
        }
    }
}