namespace StatsDownload.Database.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;
    using Core.Interfaces.Logging;
    using NSubstitute;
    using NSubstitute.ClearExtensions;
    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadDatabaseProvider
    {
        [SetUp]
        public void SetUp()
        {
            databaseConnectionServiceMock = Substitute.For<IDatabaseConnectionService>();

            statsDownloadDatabaseServiceMock = Substitute.For<IStatsDownloadDatabaseService>();
            statsDownloadDatabaseServiceMock.When(service =>
                service.CreateDatabaseConnectionAndExecuteAction(
                    Arg.Any<Action<IDatabaseConnectionService>>())).Do(
                callInfo =>
                {
                    var service = callInfo.Arg<Action<IDatabaseConnectionService>>();

                    service.Invoke(databaseConnectionServiceMock);
                });

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewFileDownloadDatabaseProvider(statsDownloadDatabaseServiceMock,
                loggingServiceMock);

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

            filePayload = new FilePayload
            {
                DownloadId = 100,
                DecompressedDownloadFileName = "DecompressedDownloadFileName",
                DecompressedDownloadFileExtension = "DecompressedDownloadFileExtension",
                DecompressedDownloadFileData = "DecompressedDownloadFileData"
            };

            fileDownloadResult = new FileDownloadResult(filePayload);

            dbDataReaderMock = Substitute.For<DbDataReader>();
            dbDataReaderMock.Read().Returns(true, true, true, false);
            dbDataReaderMock.GetInt32(0).Returns(100, 200, 300);

            databaseConnectionServiceMock
                .ExecuteReader("SELECT DownloadId FROM [FoldingCoin].[DownloadsReadyForUpload]")
                .Returns(dbDataReaderMock);

            downloadIdParameterMock = Substitute.For<DbParameter>();
            downloadIdParameterMock.Value.Returns(100);
            statsDownloadDatabaseServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock, 100)
                                            .Returns(downloadIdParameterMock);
            statsDownloadDatabaseServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock)
                                            .Returns(downloadIdParameterMock);
            statsDownloadDatabaseServiceMock
                .CreateDownloadIdParameter(databaseConnectionServiceMock, ParameterDirection.Output)
                .Returns(downloadIdParameterMock);

            errorMessageParameterMock = Substitute.For<DbParameter>();
        }

        private const int NumberOfRowsEffectedExpected = 5;

        private IDatabaseConnectionService databaseConnectionServiceMock;

        private DbDataReader dbDataReaderMock;

        private DbParameter downloadIdParameterMock;

        private DbParameter errorMessageParameterMock;

        private FileDownloadResult fileDownloadResult;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private IStatsDownloadDatabaseService statsDownloadDatabaseServiceMock;

        private IFileDownloadDatabaseService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDatabaseProvider(null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadDatabaseProvider(statsDownloadDatabaseServiceMock, null));
        }

        [Test]
        public void FileDownloadError_WhenInvoked_ParametersAreProvided()
        {
            fileDownloadResult = new FileDownloadResult(FailedReason.UnexpectedException, filePayload);

            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                                             service =>
                                                 service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadError]",
                                                     Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            statsDownloadDatabaseServiceMock.CreateErrorMessageParameter(databaseConnectionServiceMock,
                fileDownloadResult).Returns(errorMessageParameterMock);

            InvokeFileDownloadError();

            Assert.That(actualParameters.Count, Is.EqualTo(2));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            Assert.That(actualParameters[1], Is.EqualTo(errorMessageParameterMock));
        }

        [Test]
        public void FileDownloadError_WhenInvoked_UpdatesFileDownloadToError()
        {
            InvokeFileDownloadError();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.FileDownloadError));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadError]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void FileDownloadFinished_WhenInvoked_FileDataUpload()
        {
            InvokeFileDownloadFinished();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.FileDownloadFinished));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadFinished]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void FileDownloadFinished_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                                             service =>
                                                 service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadFinished]",
                                                     Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeFileDownloadFinished();

            Assert.That(actualParameters.Count, Is.EqualTo(4));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FileName"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("DecompressedDownloadFileName"));
            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FileExtension"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[2].Value, Is.EqualTo("DecompressedDownloadFileExtension"));
            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@FileData"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[3].Value, Is.EqualTo("DecompressedDownloadFileData"));
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenInvoked_GetsLastfileDownloadDateTime()
        {
            InvokeGetLastFileFownloadDateTime();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.GetLastFileDownloadDateTime));
                databaseConnectionServiceMock.ExecuteScalar("SELECT [FoldingCoin].[GetLastFileDownloadDateTime]()");
            });
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenInvoked_ReturnsDateTime()
        {
            DateTime dateTime = DateTime.UtcNow;
            databaseConnectionServiceMock.ExecuteScalar("SELECT [FoldingCoin].[GetLastFileDownloadDateTime]()")
                                         .Returns(dateTime);

            DateTime actual = InvokeGetLastFileFownloadDateTime();

            Assert.That(actual, Is.EqualTo(dateTime));
        }

        [Test]
        public void GetLastFileDownloadDateTime_WhenNoRowsReturned_ReturnsDefaultDateTime()
        {
            DateTime actual = InvokeGetLastFileFownloadDateTime();

            Assert.That(actual, Is.EqualTo(default(DateTime)));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsAvailable_WhenInvoked_ReturnsDatabaseAvailability(bool expected)
        {
            statsDownloadDatabaseServiceMock.IsAvailable().Returns(expected);

            bool actual = InvokeIsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_NewFileDownloadStarted()
        {
            InvokeNewFileDownloadStarted();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.NewFileDownloadStarted));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[NewFileDownloadStarted]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock.When(
                                             service =>
                                                 service.ExecuteStoredProcedure(
                                                     "[FoldingCoin].[NewFileDownloadStarted]",
                                                     Arg.Any<List<DbParameter>>()))
                                         .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeNewFileDownloadStarted();

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            statsDownloadDatabaseServiceMock
                .Received().CreateDownloadIdParameter(databaseConnectionServiceMock, ParameterDirection.Output);
        }

        [Test]
        public void NewFileDownloadStarted_WhenInvoked_ReturnsDownloadId()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns(100);

            databaseConnectionServiceMock.ClearSubstitute();
            statsDownloadDatabaseServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock)
                                            .Returns(dbParameter);

            InvokeNewFileDownloadStarted();

            Assert.That(filePayload.DownloadId, Is.EqualTo(100));
        }

        [Test]
        public void UpdateToLatest_WhenInvoked_DatabaseUpdatedToLatest()
        {
            databaseConnectionServiceMock.ExecuteStoredProcedure(Arg.Any<string>())
                                         .Returns(NumberOfRowsEffectedExpected);

            InvokeUpdateToLatest();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.UpdateToLatest));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[UpdateToLatest]");
                loggingServiceMock.LogVerbose($"'{NumberOfRowsEffectedExpected}' rows were effected");
            });
        }

        private void InvokeFileDownloadError()
        {
            systemUnderTest.FileDownloadError(fileDownloadResult);
        }

        private void InvokeFileDownloadFinished()
        {
            systemUnderTest.FileDownloadFinished(filePayload);
        }

        private DateTime InvokeGetLastFileFownloadDateTime()
        {
            return systemUnderTest.GetLastFileDownloadDateTime();
        }

        private bool InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private void InvokeNewFileDownloadStarted()
        {
            systemUnderTest.NewFileDownloadStarted(filePayload);
        }

        private void InvokeUpdateToLatest()
        {
            systemUnderTest.UpdateToLatest();
        }

        private IFileDownloadDatabaseService NewFileDownloadDatabaseProvider(
            IStatsDownloadDatabaseService statsDownloadDatabaseService, ILoggingService loggingService)
        {
            return new FileDownloadDatabaseProvider(statsDownloadDatabaseService,
                loggingService);
        }
    }
}