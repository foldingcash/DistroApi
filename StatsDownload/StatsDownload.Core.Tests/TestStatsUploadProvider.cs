namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;

    using NSubstitute;
    using NSubstitute.ExceptionExtensions;

    using NUnit.Framework;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    [TestFixture]
    public class TestStatsUploadProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsUploadDatabaseServiceMock = Substitute.For<IStatsUploadDatabaseService>();
            statsUploadDatabaseServiceMock.IsAvailable().Returns((true, FailedReason.None));
            statsUploadDatabaseServiceMock.GetDownloadsReadyForUpload().Returns(new List<int> { 1, 2 });
            statsUploadDatabaseServiceMock.GetFileData(1).Returns("File1");
            statsUploadDatabaseServiceMock.GetFileData(2).Returns("File2");

            loggingServiceMock = Substitute.For<IStatsUploadLoggingService>();

            downloadDateTime1 = DateTime.UtcNow;
            downloadDateTime2 = DateTime.UtcNow;

            user1 = new UserData();
            user2 = new UserData();
            user3 = new UserData();
            user4 = new UserData();

            failedUser1 = new FailedUserData(3, RejectionReason.UnexpectedFormat, new UserData());
            failedUser2 = new FailedUserData();
            failedUser3 = new FailedUserData();
            failedUser4 = new FailedUserData();

            statsFileParserServiceMock = Substitute.For<IStatsFileParserService>();
            statsFileParserServiceMock.Parse(new FilePayload { DecompressedDownloadFileData = "File1" }).Returns(
                new ParseResults(downloadDateTime1, new List<UserData> { user1, user2 },
                    new List<FailedUserData> { failedUser1, failedUser2 }));
            statsFileParserServiceMock.Parse(new FilePayload { DecompressedDownloadFileData = "File2" }).Returns(
                new ParseResults(downloadDateTime2, new List<UserData> { user3, user4 },
                    new List<FailedUserData> { failedUser3, failedUser4 }));

            statsUploadEmailServiceMock = Substitute.For<IStatsUploadEmailService>();

            systemUnderTest = NewStatsUploadProvider(statsUploadDatabaseServiceMock, loggingServiceMock,
                statsFileParserServiceMock, statsUploadEmailServiceMock);
        }

        private DateTime downloadDateTime1;

        private DateTime downloadDateTime2;

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
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsUploadProvider(null, loggingServiceMock, statsFileParserServiceMock,
                    statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadProvider(statsUploadDatabaseServiceMock, null,
                statsFileParserServiceMock, statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadProvider(statsUploadDatabaseServiceMock,
                loggingServiceMock, null, statsUploadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsUploadProvider(statsUploadDatabaseServiceMock,
                loggingServiceMock, statsFileParserServiceMock, null));
        }

        [Test]
        public void UploadStatsFiles_WhenDatabaseIsNotAvailable_LogsResult()
        {
            SetUpWhenDatabaseIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            loggingServiceMock.Received().LogResults(actual);
        }

        [Test]
        public void UploadStatsFiles_WhenDatabaseIsNotAvailable_ReturnsDatabaseUnavailableResult()
        {
            SetUpWhenDatabaseIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DatabaseUnavailable));
        }

        [Test]
        public void UploadStatsFiles_WhenDatabaseIsNotAvailable_SendsEmail()
        {
            SetUpWhenDatabaseIsNotAvailable();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void UploadStatsFiles_WhenDatabaseTimeoutExceptionThrown_ReturnsDatabaseTimeoutResult()
        {
            SetUpWhenDatabaseTimeoutExceptionThrown();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.UploadResults.ElementAt(0).DownloadId, Is.EqualTo(1));
            Assert.That(actual.UploadResults.ElementAt(0).FailedReason,
                Is.EqualTo(FailedReason.UnexpectedDatabaseException));
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionBeforeAnUploadStarts_LogsException()
        {
            Exception expected = SetUpWhenExceptionBeforeAnUploadStarts();

            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("UploadStatsFiles Invoked");
                statsUploadDatabaseServiceMock.IsAvailable();
                loggingServiceMock.LogException(expected);
            });
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionBeforeAnUploadStarts_ReturnsUnexpectedExceptionResult()
        {
            SetUpWhenExceptionBeforeAnUploadStarts();

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionBeforeAnUploadStartsThrown_SendsEmail()
        {
            SetUpWhenExceptionBeforeAnUploadStarts();

            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock.Received().SendEmail(Arg.Any<StatsUploadResults>());
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrown_LogsException()
        {
            var expected = new Exception();
            statsFileParserServiceMock.Parse(Arg.Any<FilePayload>()).Throws(expected);

            InvokeUploadStatsFiles();

            loggingServiceMock.Received(2).LogException(expected);
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrown_ResultInvalidStatFileData()
        {
            statsFileParserServiceMock.Parse(Arg.Any<FilePayload>()).Throws(new Exception());

            StatsUploadResults actual = InvokeUploadStatsFiles();

            Assert.That(actual.UploadResults.ElementAt(0).DownloadId, Is.EqualTo(1));
            Assert.That(actual.UploadResults.ElementAt(0).FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
        }

        [Test]
        public void UploadStatsFiles_WhenExceptionThrownAfterAnUploadStarts_RollsBackTransaction()
        {
            statsUploadDatabaseServiceMock.WhenForAnyArgs(service => service.AddUsers(null, 0, null, null))
                                          .Do(info => throw new Exception());

            var tranaction = Substitute.For<DbTransaction>();
            statsUploadDatabaseServiceMock.CreateTransaction().Returns(tranaction);

            InvokeUploadStatsFiles();

            statsUploadDatabaseServiceMock.Received().Rollback(tranaction);
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
        [Ignore("May not be needed")]
        public void UploadStatsFiles_WhenInvoked_AddsUsersData()
        {
            var transaction1 = Substitute.For<DbTransaction>();
            var transaction2 = Substitute.For<DbTransaction>();
            statsUploadDatabaseServiceMock.CreateTransaction().Returns(transaction1, transaction2);

            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                statsUploadDatabaseServiceMock.AddUsers(transaction1, 1,
                    Arg.Is<IEnumerable<UserData>>(datas => datas.Contains(user1) && datas.Contains(user2)),
                    Arg.Is<IList<FailedUserData>>(data => data.Contains(failedUser1) && data.Contains(failedUser2)));
                statsUploadDatabaseServiceMock.AddUsers(transaction2, 2,
                    Arg.Is<IEnumerable<UserData>>(datas => datas.Contains(user3) && datas.Contains(user4)),
                    Arg.Is<IList<FailedUserData>>(data => data.Contains(failedUser3) && data.Contains(failedUser4)));
            });
        }

        [Test]
        [Ignore("May not be needed")]
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
        [Ignore("May not be needed")]
        public void UploadStatsFiles_WhenInvoked_UpdatesTheDownloadFileState()
        {
            var transaction1 = Substitute.For<DbTransaction>();
            var transaction2 = Substitute.For<DbTransaction>();
            statsUploadDatabaseServiceMock.CreateTransaction().Returns(transaction1, transaction2);

            InvokeUploadStatsFiles();

            Received.InOrder(() =>
            {
                statsUploadDatabaseServiceMock.GetFileData(1);
                statsUploadDatabaseServiceMock.StartStatsUpload(transaction1, 1, downloadDateTime1);
                statsUploadDatabaseServiceMock.StatsUploadFinished(transaction1, 1);
                statsUploadDatabaseServiceMock.Commit(transaction1);
                statsUploadDatabaseServiceMock.GetFileData(2);
                statsUploadDatabaseServiceMock.StartStatsUpload(transaction2, 2, downloadDateTime2);
                statsUploadDatabaseServiceMock.StatsUploadFinished(transaction2, 2);
                statsUploadDatabaseServiceMock.Commit(transaction2);
            });
        }

        [Test]
        [Ignore("May not be needed")]
        public void UploadStatsFiles_WhenParsingUserDataFails_ErrorsLogged()
        {
            InvokeUploadStatsFiles();

            loggingServiceMock.Received(2).LogFailedUserData(1, Arg.Any<FailedUserData>());
            loggingServiceMock.Received(2).LogFailedUserData(2, Arg.Any<FailedUserData>());
        }

        [Test]
        [Ignore("May not be needed")]
        public void UploadStatsFiles_WhenParsingUserDataFails_SendsEmail()
        {
            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock
                .Received(1).SendEmail(Arg.Is<IList<FailedUserData>>(data => data.Contains(failedUser2)));
            statsUploadEmailServiceMock
                .Received(1)
                .SendEmail(Arg.Is<IList<FailedUserData>>(data =>
                    data.Contains(failedUser3) && data.Contains(failedUser4)));
        }

        [Test]
        public void UploadStatsFiles_WhenUnexpectedFormat_DoesNotSendEmail()
        {
            InvokeUploadStatsFiles();

            statsUploadEmailServiceMock
                .DidNotReceive().SendEmail(Arg.Is<IList<FailedUserData>>(data => data.Contains(failedUser1)));
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
                statsFileParserService, statsUploadEmailService);
        }

        private void SetUpWhenDatabaseIsNotAvailable()
        {
            statsUploadDatabaseServiceMock.IsAvailable().Returns((false, FailedReason.DatabaseUnavailable));
        }

        private void SetUpWhenDatabaseTimeoutExceptionThrown()
        {
            var exception = new TestDbTimeoutException();
            statsUploadDatabaseServiceMock.GetFileData(1).Throws(exception);
        }

        private Exception SetUpWhenExceptionBeforeAnUploadStarts()
        {
            var exception = new Exception();
            statsUploadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => throw exception);
            return exception;
        }

        private void SetUpWhenInvalidStatsFileExceptionThrown()
        {
            var exception = new InvalidStatsFileException();
            statsFileParserServiceMock.Parse(Arg.Any<FilePayload>()).Throws(exception);
        }

        private class TestDbTimeoutException : DbException
        {
            public TestDbTimeoutException()
                : base("Mock timeout exception")
            {
            }
        }
    }
}