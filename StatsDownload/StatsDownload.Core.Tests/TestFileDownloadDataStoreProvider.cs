namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadDataStoreProvider
    {
        private const int NumberOfRowsEffectedExpected = 5;

        private IDatabaseConnectionServiceFactory databaseConnectionServiceFactoryMock;

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private IDatabaseConnectionSettingsService databaseConnectionSettingsServiceMock;

        private IFileDownloadLoggingService fileDownloadLoggingServiceMock;

        private IFileDownloadDataStoreService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(
                    null,
                    databaseConnectionServiceFactoryMock,
                    fileDownloadLoggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(
                    databaseConnectionSettingsServiceMock,
                    null,
                    fileDownloadLoggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadDataStoreProvider(
                    databaseConnectionSettingsServiceMock,
                    databaseConnectionServiceFactoryMock,
                    null));
        }

        [Test]
        public void IsAvailable_Invoked_DependenciesCalledInOrder()
        {
            InvokeIsAvailable();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("IsAvailable Invoked");
                        databaseConnectionServiceMock.Open();
                        fileDownloadLoggingServiceMock.LogVerbose("Database connection was successful");
                        databaseConnectionServiceMock.Close();
                    }));
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_DependenciesCalledInOrder()
        {
            var expected = new Exception();
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw(expected);

            InvokeIsAvailable();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("IsAvailable Invoked");
                        databaseConnectionServiceMock.Open();
                        databaseConnectionServiceMock.Close();
                        fileDownloadLoggingServiceMock.LogException(expected);
                    }));
        }

        [Test]
        public void IsAvailable_WhenDatabaseConnectionFails_ReturnsFalse()
        {
            databaseConnectionServiceMock.When(mock => mock.Open()).Throw<Exception>();

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
        public void NewFileDownloadStarted_WhenInvoked_DependenciesCalledInOrder()
        {
            InvokeNewFileDownloadStarted();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("NewFileDownloadStarted Invoked");
                        databaseConnectionServiceMock.Open();
                        fileDownloadLoggingServiceMock.LogVerbose("Database connection was successful");
                        databaseConnectionServiceMock.ExecuteStoredProcedure(
                            "[FoldingCoin].[NewFileDownloadStarted]",
                            Arg.Any<List<DbParameter>>());
                        databaseConnectionServiceMock.Close();
                    }));
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                service =>
                service.ExecuteStoredProcedure("[FoldingCoin].[NewFileDownloadStarted]", Arg.Any<List<DbParameter>>()))
                .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeNewFileDownloadStarted();

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0].ParameterName, Is.EqualTo("@DownloadId"));
            Assert.That(actualParameters[0].DbType, Is.EqualTo(DbType.Int32));
            Assert.That(actualParameters[0].Direction, Is.EqualTo(ParameterDirection.Output));
        }

        [SetUp]
        public void SetUp()
        {
            databaseConnectionSettingsServiceMock = Substitute.For<IDatabaseConnectionSettingsService>();
            databaseConnectionSettingsServiceMock.GetConnectionString().Returns("connectionString");

            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();
            databaseConnectionServiceFactoryMock = Substitute.For<IDatabaseConnectionServiceFactory>();
            databaseConnectionServiceFactoryMock.Create("connectionString").Returns(databaseConnectionServiceMock);

            fileDownloadLoggingServiceMock = Substitute.For<IFileDownloadLoggingService>();

            systemUnderTest = NewFileDownloadDataStoreProvider(
                databaseConnectionSettingsServiceMock,
                databaseConnectionServiceFactoryMock,
                fileDownloadLoggingServiceMock);

            databaseConnectionServiceMock.CreateParameter(
                Arg.Any<string>(),
                Arg.Any<DbType>(),
                Arg.Any<ParameterDirection>()).Returns(
                    info =>
                        {
                            var parameterName = info.Arg<string>();
                            var dbType = info.Arg<DbType>();
                            var direction = info.Arg<ParameterDirection>();

                            var dbParameter = Substitute.For<DbParameter>();
                            dbParameter.ParameterName.Returns(parameterName);
                            dbParameter.DbType.Returns(dbType);
                            dbParameter.Direction.Returns(direction);

                            return dbParameter;
                        });
        }

        [Test]
        public void UpdateToLatest_WhenInvoked_DependenciesCalledInOrder()
        {
            databaseConnectionServiceMock.ExecuteStoredProcedure(Arg.Any<string>())
                .Returns(NumberOfRowsEffectedExpected);

            InvokeUpdateToLatest();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("UpdateToLatest Invoked");
                        databaseConnectionServiceMock.Open();
                        fileDownloadLoggingServiceMock.LogVerbose("Database connection was successful");
                        databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[UpdateToLatest]");
                        fileDownloadLoggingServiceMock.LogVerbose(
                            $"'{NumberOfRowsEffectedExpected}' rows were effected");
                        databaseConnectionServiceMock.Close();
                    }));
        }

        private bool InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private void InvokeNewFileDownloadStarted()
        {
            systemUnderTest.NewFileDownloadStarted();
        }

        private void InvokeUpdateToLatest()
        {
            systemUnderTest.UpdateToLatest();
        }

        private IFileDownloadDataStoreService NewFileDownloadDataStoreProvider(
            IDatabaseConnectionSettingsService databaseConnectionSettingsService,
            IDatabaseConnectionServiceFactory databaseConnectionServiceFactory,
            IFileDownloadLoggingService fileDownloadLoggingService)
        {
            return new FileDownloadDataStoreProvider(
                databaseConnectionSettingsService,
                databaseConnectionServiceFactory,
                fileDownloadLoggingService);
        }
    }
}