namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsUploadProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsUploadDatabaseServiceMock = Substitute.For<IStatsUploadDatabaseService>();
            statsUploadDatabaseServiceMock.IsAvailable().Returns(true);
            statsUploadDatabaseServiceMock.GetDownloadsReadyForUpload().Returns(new List<int> { 1, 2 });
            statsUploadDatabaseServiceMock.GetFileData(1).Returns("File1");
            statsUploadDatabaseServiceMock.GetFileData(2).Returns("File2");

            loggingServiceMock = Substitute.For<IStatsUploadLoggingService>();

            downloadDateTime = DateTime.Now;

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
                                      .Returns(new ParseResults(downloadDateTime, new List<UserData> { user1, user2 },
                                          new List<FailedUserData> { failedUser1, failedUser2 }));
            statsFileParserServiceMock.Parse("File2")
                                      .Returns(new ParseResults(downloadDateTime, new List<UserData> { user3, user4 },
                                          new List<FailedUserData> { failedUser3, failedUser4 }));

            statsUploadEmailServiceMock = Substitute.For<IStatsUploadEmailService>();

            systemUnderTest = NewStatsUploadProvider(statsUploadDatabaseServiceMock, loggingServiceMock,
                statsFileParserServiceMock, statsUploadEmailServiceMock);
        }

        private DateTime downloadDateTime;

        private FailedUserData failedUser1;

        private FailedUserData failedUser2;

        private FailedUserData failedUser3;

        private FailedUserData failedUser4;

        private IStatsUploadLoggingService loggingServiceMock;

        private IStatsFileParserService statsFileParserServiceMock;

        private IStatsUploadDatabaseService statsUploadDatabaseServiceMock;

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
                    NewStatsUploadProvider(null, loggingServiceMock, statsFileParserServiceMock,
                        statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewStatsUploadProvider(statsUploadDatabaseServiceMock, null, statsFileParserServiceMock,
                        statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewStatsUploadProvider(statsUploadDatabaseServiceMock, loggingServiceMock, null,
                        statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewStatsUploadProvider(statsUploadDatabaseServiceMock, loggingServiceMock,
                        statsFileParserServiceMock,
                        null));
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
                statsUploadDatabaseServiceMock.IsAvailable();
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

            Assert.That(actual.UploadResults.ElementAt(0).DownloadId, Is.EqualTo(1));
            Assert.That(actual.UploadResults.ElementAt(0).FailedReason,
                Is.EqualTo(FailedReason.InvalidStatsFileUpload));
        }

        [Test]
        public void UploadStatsFiles_WhenInvalidStatsFileExceptionThrown_UpdatesStatsUploadToError()
        {
            SetUpWhenInvalidStatsFileExceptionThrown();

            InvokeUploadStatsFiles();

            statsUploadDatabaseServiceMock.Received().StatsUploadError(Arg.Any<StatsUploadResult>());
        }

        [Test]
        public void UploadStatsFiles_WhenInvoked_AddsUsersData()
        {
            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                statsUploadDatabaseServiceMock.AddUsers(1,
                    Arg.Is<IEnumerable<UserData>>(datas => datas.Contains(user1) && datas.Contains(user2)),
                    Arg.Is<IEnumerable<FailedUserData>>(
                        data => data.Contains(failedUser1) && data.Contains(failedUser2)));
                statsUploadDatabaseServiceMock.AddUsers(2,
                    Arg.Is<IEnumerable<UserData>>(datas => datas.Contains(user3) && datas.Contains(user4)),
                    Arg.Is<IEnumerable<FailedUserData>>(
                        data => data.Contains(failedUser3) && data.Contains(failedUser4)));
            });
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
                statsUploadDatabaseServiceMock.StartStatsUpload(1);
                statsUploadDatabaseServiceMock.GetFileData(1);
                statsUploadDatabaseServiceMock.StatsUploadFinished(1, downloadDateTime);
                statsUploadDatabaseServiceMock.StartStatsUpload(2);
                statsUploadDatabaseServiceMock.GetFileData(2);
                statsUploadDatabaseServiceMock.StatsUploadFinished(2, downloadDateTime);
            });
        }

        [Test]
        public void UploadStatsFiles_WhenParsingUserDataFails_ErrorsLogged()
        {
            InvokeUploadStatsFiles();

            loggingServiceMock.Received(2).LogFailedUserData(1, Arg.Any<FailedUserData>());
            loggingServiceMock.Received(2).LogFailedUserData(2, Arg.Any<FailedUserData>());
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

        private IStatsUploadService NewStatsUploadProvider(IStatsUploadDatabaseService statsUploadDatabaseService,
            IStatsUploadLoggingService statsUploadLoggingService,
            IStatsFileParserService statsFileParserService,
            IStatsUploadEmailService statsUploadEmailService)
        {
            return new StatsUploadProvider(statsUploadDatabaseService, statsUploadLoggingService,
                statsFileParserService,
                statsUploadEmailService);
        }

        private void SetUpWhenDataStoreIsNotAvailable()
        {
            statsUploadDatabaseServiceMock.IsAvailable().Returns(false);
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var exception = new Exception();
            statsUploadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw exception; });
            return exception;
        }

        private void SetUpWhenInvalidStatsFileExceptionThrown()
        {
            var exception = new InvalidStatsFileException();
            statsFileParserServiceMock.Parse(Arg.Any<string>()).Throws(exception);
        }
    }
}