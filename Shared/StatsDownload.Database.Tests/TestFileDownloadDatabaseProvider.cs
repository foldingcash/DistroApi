namespace StatsDownload.Database.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using NSubstitute;
    using NSubstitute.ClearExtensions;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;

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
                    Arg.Any<Action<IDatabaseConnectionService>>())).Do(callInfo =>
            {
                var service = callInfo.Arg<Action<IDatabaseConnectionService>>();

                service.Invoke(databaseConnectionServiceMock);
            });

            statsDownloadDatabaseParameterServiceMock = Substitute.For<IStatsDownloadDatabaseParameterService>();

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewFileDownloadDatabaseProvider(statsDownloadDatabaseServiceMock,
                statsDownloadDatabaseParameterServiceMock, loggingServiceMock);

            DatabaseProviderTestingHelper.SetUpDatabaseConnectionServiceReturns(databaseConnectionServiceMock);

            filePayload = new FilePayload
                          {
                              DownloadId = 100,
                              UploadPath = "UploadPath",
                              DownloadFileName = "DownloadFileName",
                              DownloadFileExtension = "DownloadFileExtension"
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
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock, 100)
                                                     .Returns(downloadIdParameterMock);
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock)
                                                     .Returns(downloadIdParameterMock);
            statsDownloadDatabaseParameterServiceMock
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

        private IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterServiceMock;

        private IStatsDownloadDatabaseService statsDownloadDatabaseServiceMock;

        private IFileDownloadDatabaseService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadDatabaseProvider(null, statsDownloadDatabaseParameterServiceMock, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadDatabaseProvider(statsDownloadDatabaseServiceMock, null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadDatabaseProvider(statsDownloadDatabaseServiceMock,
                statsDownloadDatabaseParameterServiceMock, null));
        }

        [Test]
        public void FileDownloadError_WhenInvoked_ParametersAreProvided()
        {
            fileDownloadResult = new FileDownloadResult(FailedReason.UnexpectedException, filePayload);

            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock
                .When(service =>
                    service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadError]", Arg.Any<List<DbParameter>>()))
                .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            statsDownloadDatabaseParameterServiceMock
                .CreateErrorMessageParameter(databaseConnectionServiceMock, fileDownloadResult)
                .Returns(errorMessageParameterMock);

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

            databaseConnectionServiceMock
                .When(service =>
                    service.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadFinished]",
                        Arg.Any<List<DbParameter>>())).Do(callback =>
                {
                    actualParameters = callback.Arg<List<DbParameter>>();
                });

            InvokeFileDownloadFinished();

            Assert.That(actualParameters.Count, Is.EqualTo(4));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            Assert.That(actualParameters[1].ParameterName, Is.EqualTo("@FilePath"));
            Assert.That(actualParameters[1].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[1].Value, Is.EqualTo("UploadPath"));
            Assert.That(actualParameters[2].ParameterName, Is.EqualTo("@FileName"));
            Assert.That(actualParameters[2].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[2].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[2].Value, Is.EqualTo("DownloadFileName"));
            Assert.That(actualParameters[3].ParameterName, Is.EqualTo("@FileExtension"));
            Assert.That(actualParameters[3].DbType, Is.EqualTo(DbType.String));
            Assert.That(actualParameters[3].Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(actualParameters[3].Value, Is.EqualTo("DownloadFileExtension"));
        }

        [Test]
        public void FileDownloadStarted_WhenInvoked_FileDownloadStarted()
        {
            InvokeFileDownloadStarted();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogMethodInvoked(nameof(systemUnderTest.FileDownloadStarted));
                databaseConnectionServiceMock.ExecuteStoredProcedure("[FoldingCoin].[FileDownloadStarted]",
                    Arg.Any<List<DbParameter>>());
            });
        }

        [Test]
        public void FileDownloadStarted_WhenInvoked_ParametersAreProvided()
        {
            List<DbParameter> actualParameters = default(List<DbParameter>);

            databaseConnectionServiceMock
                .When(service =>
                    service.ExecuteStoredProcedure("[FoldingCoin].FileDownloadStarted]", Arg.Any<List<DbParameter>>()))
                .Do(callback => { actualParameters = callback.Arg<List<DbParameter>>(); });

            InvokeFileDownloadStarted();

            Assert.That(actualParameters.Count, Is.EqualTo(1));
            Assert.That(actualParameters[0], Is.EqualTo(downloadIdParameterMock));
            statsDownloadDatabaseParameterServiceMock
                .Received().CreateDownloadIdParameter(databaseConnectionServiceMock, ParameterDirection.Output);
        }

        [Test]
        public void FileDownloadStarted_WhenInvoked_ReturnsDownloadId()
        {
            var dbParameter = Substitute.For<DbParameter>();
            dbParameter.Value.Returns(100);

            databaseConnectionServiceMock.ClearSubstitute();
            statsDownloadDatabaseParameterServiceMock.CreateDownloadIdParameter(databaseConnectionServiceMock)
                                                     .Returns(dbParameter);

            InvokeFileDownloadStarted();

            Assert.That(filePayload.DownloadId, Is.EqualTo(100));
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

        [TestCase(true, DatabaseFailedReason.None, FailedReason.None)]
        [TestCase(false, DatabaseFailedReason.DatabaseUnavailable, FailedReason.DatabaseUnavailable)]
        public void IsAvailable_WhenInvoked_ReturnsDatabaseAvailability(bool expectedIsAvailable,
                                                                        DatabaseFailedReason failedReason,
                                                                        FailedReason expectedReason)
        {
            statsDownloadDatabaseServiceMock.IsAvailable(Constants.FileDownloadDatabase.FileDownloadObjects)
                                            .Returns((expectedIsAvailable, failedReason));

            (bool isAvailable, FailedReason reason) actual = InvokeIsAvailable();

            Assert.That(actual.isAvailable, Is.EqualTo(expectedIsAvailable));
            Assert.That(actual.reason, Is.EqualTo(expectedReason));
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

        private void InvokeFileDownloadStarted()
        {
            systemUnderTest.FileDownloadStarted(filePayload);
        }

        private DateTime InvokeGetLastFileFownloadDateTime()
        {
            return systemUnderTest.GetLastFileDownloadDateTime();
        }

        private (bool isAvailable, FailedReason reason) InvokeIsAvailable()
        {
            return systemUnderTest.IsAvailable();
        }

        private void InvokeUpdateToLatest()
        {
            systemUnderTest.UpdateToLatest();
        }

        private IFileDownloadDatabaseService NewFileDownloadDatabaseProvider(
            IStatsDownloadDatabaseService statsDownloadDatabaseService,
            IStatsDownloadDatabaseParameterService statsDownloadDatabaseParameterService,
            ILoggingService loggingService)
        {
            return new FileDownloadDatabaseProvider(statsDownloadDatabaseService, statsDownloadDatabaseParameterService,
                loggingService);
        }
    }
}