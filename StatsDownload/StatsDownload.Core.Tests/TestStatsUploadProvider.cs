namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    [TestFixture]
    public class TestStatsUploadProvider
    {
        private FailedUserData failedUser1;

        private FailedUserData failedUser2;

        private FailedUserData failedUser3;

        private FailedUserData failedUser4;

        private IStatsUploadLoggingService loggingServiceMock;

        private IStatsFileParserService statsFileParserServiceMock;

        private IStatsUploadDataStoreService statsUploadDataStoreServiceMock;

        private IStatsUploadEmailService statsUploadEmailServiceMock;

        private IStatsUploadService systemUnderTest;

        private UserData user1;

        private UserData user2;

        private UserData user3;

        private UserData user4;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewStatsUploadProvider(null, loggingServiceMock, statsFileParserServiceMock, statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewStatsUploadProvider(statsUploadDataStoreServiceMock, null, statsFileParserServiceMock,
                    statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewStatsUploadProvider(statsUploadDataStoreServiceMock, loggingServiceMock, null,
                    statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewStatsUploadProvider(statsUploadDataStoreServiceMock, loggingServiceMock, statsFileParserServiceMock,
                    null));
        }

        [SetUp]
        public void SetUp()
        {
            statsUploadDataStoreServiceMock = Substitute.For<IStatsUploadDataStoreService>();
            statsUploadDataStoreServiceMock.IsAvailable().Returns(true);
            statsUploadDataStoreServiceMock.GetDownloadsReadyForUpload().Returns(new List<int> { 1, 2 });
            statsUploadDataStoreServiceMock.GetFileData(1).Returns("File1");
            statsUploadDataStoreServiceMock.GetFileData(2).Returns("File2");

            loggingServiceMock = Substitute.For<IStatsUploadLoggingService>();

            user1 = new UserData();
            user2 = new UserData();
            user3 = new UserData();
            user4 = new UserData();

            failedUser1 = new FailedUserData();
            failedUser2 = new FailedUserData();
            failedUser3 = new FailedUserData();
            failedUser4 = new FailedUserData();

            statsFileParserServiceMock = Substitute.For<IStatsFileParserService>();
            statsFileParserServiceMock.Parse("File1")
                                      .Returns(new ParseResults(new List<UserData> { user1, user2 },
                                          new List<FailedUserData> { failedUser1, failedUser2 }));
            statsFileParserServiceMock.Parse("File2")
                                      .Returns(new ParseResults(new List<UserData> { user3, user4 },
                                          new List<FailedUserData> { failedUser3, failedUser4 }));

            statsUploadEmailServiceMock = Substitute.For<IStatsUploadEmailService>();

            systemUnderTest = NewStatsUploadProvider(statsUploadDataStoreServiceMock, loggingServiceMock,
                statsFileParserServiceMock, statsUploadEmailServiceMock);
        }

        [Test]
        public void UploadStatsFiles_WhenDataStoreIsNotAvailable_LogsResult()
        {
            SetUpWhenDataStoreIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            loggingServiceMock.Received().LogResults(actual);
        }

        [Test]
        public void UploadStatsFiles_WhenDataStoreIsNotAvailable_ReturnsDataStoreUnavailableResult()
        {
            SetUpWhenDataStoreIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
        }

        [Test]
        public void UploadStatsFiles_WhenDataStoreIsNotAvailable_SendsEmail()
        {
            SetUpWhenDataStoreIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrown_LogsException()
        {
            Exception expected = SetUpWhenExceptionThrown();

            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("UploadStatsFiles Invoked");
                statsUploadDataStoreServiceMock.IsAvailable();
                loggingServiceMock.LogException(expected);
            });
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrown_ReturnsUnexpectedExceptionResult()
        {
            SetUpWhenExceptionThrown();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrown_SendsEmail()
        {
            SetUpWhenExceptionThrown();

            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received().SendEmail(Arg.Any<StatsUploadResults>());
        }

        [Test]
        public void UploadStatsFiles_WhenInvalidStatsFileExceptionThrown_EmailsResult()
        {
            SetUpWhenInvalidStatsFileExceptionThrown();

            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received().SendEmail(Arg.Any<StatsUploadResult>());
        }

        [Test]
        public void UploadStatsFiles_WhenInvalidStatsFileExceptionThrown_LogsResult()
        {
            SetUpWhenInvalidStatsFileExceptionThrown();

            InvokeUploadStatsFiles();

            loggingServiceMock.Received().LogResult(Arg.Any<StatsUploadResult>());
        }

        [Test]
        public void UploadStatsFiles_WhenInvalidStatsFileExceptionThrown_ResultInvalidStatFileData()
        {
            SetUpWhenInvalidStatsFileExceptionThrown();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.UploadResults[0].DownloadId, Is.EqualTo(1));
            Assert.That(actual.UploadResults[0].FailedReason, Is.EqualTo(FailedReason.InvalidStatsFileUpload));
        }

        [Test]
        public void UploadStatsFiles_WhenInvalidStatsFileExceptionThrown_UpdatesStatsUploadToError()
        {
            SetUpWhenInvalidStatsFileExceptionThrown();

            InvokeUploadStatsFiles();

            statsUploadDataStoreServiceMock.Received().StatsUploadError(Arg.Any<StatsUploadResult>());
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_AddsUserDataForEachDownloadReadyForUpload()
        {
            InvokeUploadStatsFiles();

            statsUploadDataStoreServiceMock.Received(1).AddUserData(user1);
            statsUploadDataStoreServiceMock.Received(1).AddUserData(user2);
            statsUploadDataStoreServiceMock.Received(1).AddUserData(user3);
            statsUploadDataStoreServiceMock.Received(1).AddUserData(user4);
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_LogsVerboseMessages()
        {
            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("UploadStatsFiles Invoked");
                loggingServiceMock.LogVerbose("Starting stats file upload. DownloadId: 1");
                loggingServiceMock.LogVerbose("Finished stats file upload. DownloadId: 1");
                loggingServiceMock.LogVerbose("Starting stats file upload. DownloadId: 2");
                loggingServiceMock.LogVerbose("Finished stats file upload. DownloadId: 2");
            });
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_ReturnsStatsUploadResults()
        {
            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.IsInstanceOf<StatsUploadResults>(actual);
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_ReturnsSuccessResult()
        {
            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.None));
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_UpdatesTheDownloadFileState()
        {
            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                statsUploadDataStoreServiceMock.StartStatsUpload(1);
                statsUploadDataStoreServiceMock.GetFileData(1);
                statsUploadDataStoreServiceMock.StatsUploadFinished(1);
                statsUploadDataStoreServiceMock.StartStatsUpload(2);
                statsUploadDataStoreServiceMock.GetFileData(2);
                statsUploadDataStoreServiceMock.StatsUploadFinished(2);
            });
        }

        [Test]
        public void UploadStatsFiles_WhenParsingUserDataFails_ErrorsLogged()
        {
            InvokeUploadStatsFiles();

            loggingServiceMock.Received(4).LogFailedUserData(Arg.Any<FailedUserData>());
        }

        [Test]
        public void UploadStatsFiles_WhenParsingUserDataFails_SendsEmail()
        {
            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received(2).SendEmail(Arg.Any<List<FailedUserData>>());
        }

        private StatsUploadResults InvokeUploadStatsFiles()
        {
            return systemUnderTest.UploadStatsFiles();
        }

        private IStatsUploadService NewStatsUploadProvider(IStatsUploadDataStoreService statsUploadDataStoreService,
                                                           IStatsUploadLoggingService statsUploadLoggingService,
                                                           IStatsFileParserService statsFileParserService,
                                                           IStatsUploadEmailService statsUploadEmailService)
        {
            return new StatsUploadProvider(statsUploadDataStoreService, statsUploadLoggingService,
                statsFileParserService, statsUploadEmailService);
        }

        private void SetUpWhenDataStoreIsNotAvailable()
        {
            statsUploadDataStoreServiceMock.IsAvailable().Returns(false);
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var exception = new Exception();
            statsUploadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw exception; });
            return exception;
        }

        private InvalidStatsFileException SetUpWhenInvalidStatsFileExceptionThrown()
        {
            var exception = new InvalidStatsFileException();
            statsFileParserServiceMock.Parse(Arg.Any<string>()).Throws(exception);
            return exception;
        }
    }
}