namespace StatsDownload.Database.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
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

            systemUnderTest = NewFileDownloadDatabaseProvider(databaseConnectionSettingsServiceMock,
                databaseConnectionServiceFactoryMock, loggingServiceMock);

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

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
                    NewFileDownloadDatabaseProvider(null, databaseConnectionServiceFactoryMock, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDatabaseProvider(databaseConnectionSettingsServiceMock, null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDatabaseProvider(databaseConnectionSettingsServiceMock,
                        databaseConnectionServiceFactoryMock, null));
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

            (bool isAvailable, FailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.False);
            Assert.That(actual.reason, Is.EqualTo(FailedReason.DatabaseUnavailable));
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsAvailable_WhenInvalidConnectionString_ReturnsFalse(string connectionString)
        {
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns(connectionString);

            (bool isAvailable, FailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.False);
            Assert.That(actual.reason, Is.EqualTo(FailedReason.DatabaseUnavailable));
        }

        [Test]
        public void IsAvailable_WhenInvoked_ReturnsTrue()
        {
            (bool isAvailable, FailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.True);
            Assert.That(actual.reason, Is.EqualTo(FailedReason.None));
        }

        [Test]
        public void Rollback_WhenInvoked_RollsBackTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Rollback(transaction);

            transaction.Received(1).Rollback();
        }

        private (bool isAvailable, FailedReason reason) InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable(null);
        }

        private IStatsDownloadDatabaseService NewFileDownloadDatabaseProvider(
            IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory, ILoggingService loggingService)
        {
            return new StatsDownloadDatabaseProvider(databaseConnectionSettingsService,
                databaseConnectionServiceFactory,
                loggingService);
        }
    }
}