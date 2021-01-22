namespace StatsDownload.Database.Tests
{
    using System;
    using System.Data;
    using System.Data.Common;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Settings;

    [TestFixture]
    public class TestStatsDownloadDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            loggingServiceMock = Substitute.For<ILoggingService>();

            databaseSettings = new DatabaseSettings { ConnectionString = "connectionString", CommandTimeout = 42 };

            databaseSettingsOptionsMock = Substitute.For<IOptions<DatabaseSettings>>();
            databaseSettingsOptionsMock.Value.Returns(databaseSettings);

            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();
            databaseConnectionServiceFactoryMock = Substitute.For<IDatabaseConnectionServiceFactory>();
            databaseConnectionServiceFactoryMock.Create().Returns(databaseConnectionServiceMock);

            systemUnderTest = NewFileDownloadDatabaseProvider(databaseSettingsOptionsMock,
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

        private DatabaseSettings databaseSettings;

        private IOptions<DatabaseSettings> databaseSettingsOptionsMock;

        private DbDataReader dbDataReaderMock;

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
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadDatabaseProvider(null, databaseConnectionServiceFactoryMock, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadDatabaseProvider(databaseSettingsOptionsMock, null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadDatabaseProvider(databaseSettingsOptionsMock, databaseConnectionServiceFactoryMock,
                    null));
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

            (bool isAvailable, DatabaseFailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.False);
            Assert.That(actual.reason, Is.EqualTo(DatabaseFailedReason.DatabaseUnavailable));
        }

        [TestCase(null)]
        [TestCase("")]
        public void IsAvailable_WhenInvalidConnectionString_ReturnsFalse(string connectionString)
        {
            databaseSettings.ConnectionString = connectionString;

            (bool isAvailable, DatabaseFailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.False);
            Assert.That(actual.reason, Is.EqualTo(DatabaseFailedReason.DatabaseUnavailable));
        }

        [Test]
        public void IsAvailable_WhenInvoked_ChecksForRequiredObjects()
        {
            InvokeIsAvailable(new[] { "object1", "object2" });

            databaseConnectionServiceMock.Received().ExecuteScalar("SELECT OBJECT_ID('object1')");
            databaseConnectionServiceMock.Received().ExecuteScalar("SELECT OBJECT_ID('object2')");
        }

        [Test]
        public void IsAvailable_WhenInvoked_ReturnsTrue()
        {
            (bool isAvailable, DatabaseFailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.True);
            Assert.That(actual.reason, Is.EqualTo(DatabaseFailedReason.None));
        }

        [Test]
        public void IsAvailable_WhenRequiredObjectsMissing_LogsMissingObjects()
        {
            databaseConnectionServiceMock.ExecuteScalar("SELECT OBJECT_ID('object1')").Returns(DBNull.Value);
            databaseConnectionServiceMock.ExecuteScalar("SELECT OBJECT_ID('object2')").Returns(1);
            databaseConnectionServiceMock.ExecuteScalar("SELECT OBJECT_ID('object3')").Returns(DBNull.Value);

            InvokeIsAvailable(new[] { "object1", "object2", "object3" });

            loggingServiceMock.Received()
                              .LogError("The required objects {'object1', 'object3'} are missing from the database.");
        }

        [Test]
        public void IsAvailable_WhenRequiredObjectsMissing_ReturnsFailedReason()
        {
            databaseConnectionServiceMock.ExecuteScalar("SELECT OBJECT_ID('object1')").Returns(DBNull.Value);

            (bool isAvailable, DatabaseFailedReason reason) actual = InvokeIsAvailable(new[] { "object1" });

            Assert.That(actual.isAvailable, Is.False);
            Assert.That(actual.reason, Is.EqualTo(DatabaseFailedReason.DatabaseMissingRequiredObjects));
        }

        [Test]
        public void Rollback_WhenInvoked_RollsBackTransaction()
        {
            var transaction = Substitute.For<DbTransaction>();

            systemUnderTest.Rollback(transaction);

            transaction.Received(1).Rollback();
        }

        private (bool isAvailable, DatabaseFailedReason reason) InvokeIsAvailable(string[] requiredObjects = null)
        {
            return systemUnderTest.IsAvailable(requiredObjects);
        }

        private IStatsDownloadDatabaseService NewFileDownloadDatabaseProvider(
            IOptions<DatabaseSettings> databaseSettingsOptions,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory, ILoggingService loggingService)
        {
            return new StatsDownloadDatabaseProvider(databaseSettingsOptions, databaseConnectionServiceFactory,
                loggingService);
        }
    }
}