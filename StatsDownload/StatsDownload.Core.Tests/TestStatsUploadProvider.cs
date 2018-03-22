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
        private IStatsUploadLoggingService loggingServiceMock;

        private IStatsFileParserService statsFileParserServiceMock;

        private IStatsUploadDataStoreService statsUploadDataStoreServiceMock;

        private IStatsUploadEmailService statsUploadEmailServiceMock;

        private IStatsUploadService systemUnderTest;

        private UserData user1;

        private UserData user2;

        private UserData user3;

        private UserData user4;

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

            statsFileParserServiceMock = Substitute.For<IStatsFileParserService>();
            statsFileParserServiceMock.Parse("File1").Returns(new List<UserData> { user1, user2 });
            statsFileParserServiceMock.Parse("File2").Returns(new List<UserData> { user3, user4 });

            statsUploadEmailServiceMock = Substitute.For<IStatsUploadEmailService>();

            systemUnderTest = new StatsUploadProvider(statsUploadDataStoreServiceMock, loggingServiceMock,
                statsFileParserServiceMock, statsUploadEmailServiceMock);
        }

        [Test]
        public void UploadStatsFiles_WhenDataStoreIsNotAvailable_ReturnsDataStoreUnavailableResult()
        {
            statsUploadDataStoreServiceMock.IsAvailable().Returns(false);

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
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

        private StatsUploadResults InvokeUploadStatsFiles()
        {
            return systemUnderTest.UploadStatsFiles();
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