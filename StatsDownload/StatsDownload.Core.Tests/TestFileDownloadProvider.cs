namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private IDownloadService downloadServiceMock;

        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFileDownloadEmailService fileDownloadEmailServiceMock;

        private IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeServiceMock;

        private IFilePayloadSettingsService filePayloadSettingsServiceMock;

        private IFilePayloadUploadService filePayloadUploadServiceMock;

        private ILoggingService loggingServiceMock;

        private IResourceCleanupService resourceCleanupServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    null,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    null,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    null,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    null,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    null,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    null,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    null,
                    filePayloadUploadServiceMock,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    null,
                    fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    resourceCleanupServiceMock,
                    fileDownloadMinimumWaitTimeServiceMock,
                    dateTimeServiceMock,
                    filePayloadUploadServiceMock,
                    null));
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_ReturnsFailedResultWithReason()
        {
            SetUpWhenDataStoreIsNotAvailable();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_SendsEmail()
        {
            SetUpWhenDataStoreIsNotAvailable();

            FileDownloadResult actual = InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ExceptionHandled()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_LogsException()
        {
            Exception expected = SetUpWhenExceptionThrown();

            InvokeDownloadFile();

            Received.InOrder(
                (() =>
                    {
                        loggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                        loggingServiceMock.LogException(expected);
                    }));
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ResourceCleanupInvoked()
        {
            SetUpWhenExceptionThrown();

            InvokeDownloadFile();

            resourceCleanupServiceMock.Received().Cleanup(Arg.Any<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_SendsEmail()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenInvoked_ResultIsSuccessAndContainsDownloadData()
        {
            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenInvoked_StatsDownloadIsPerformed()
        {
            InvokeDownloadFile();

            Received.InOrder(
                (() =>
                    {
                        loggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadDataStoreServiceMock.UpdateToLatest();
                        dateTimeServiceMock.DateTimeNow();
                        loggingServiceMock.LogVerbose($"Stats file download started: {dateTime}");
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted(Arg.Any<FilePayload>());
                        fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>());
                        filePayloadSettingsServiceMock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>());
                        downloadServiceMock.DownloadFile(Arg.Any<FilePayload>());
                        dateTimeServiceMock.DateTimeNow();
                        loggingServiceMock.LogVerbose($"Stats file download completed: {dateTime}");
                        filePayloadUploadServiceMock.UploadFile(Arg.Any<FilePayload>());
                        resourceCleanupServiceMock.Cleanup(Arg.Any<FilePayload>());
                        loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_LogsDownloadResult()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            InvokeDownloadFile();

            loggingServiceMock.Received().LogResult(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_ReturnsFailedResultWithReason()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.MinimumWaitTimeNotMet));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_SendsEmail()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            FileDownloadResult actual = InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Today;

            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            downloadServiceMock = Substitute.For<IDownloadService>();

            filePayloadSettingsServiceMock = Substitute.For<IFilePayloadSettingsService>();

            resourceCleanupServiceMock = Substitute.For<IResourceCleanupService>();

            fileDownloadMinimumWaitTimeServiceMock = Substitute.For<IFileDownloadMinimumWaitTimeService>();
            fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>()).Returns(true);

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            filePayloadUploadServiceMock = Substitute.For<IFilePayloadUploadService>();

            fileDownloadEmailServiceMock = Substitute.For<IFileDownloadEmailService>();

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                loggingServiceMock,
                downloadServiceMock,
                filePayloadSettingsServiceMock,
                resourceCleanupServiceMock,
                fileDownloadMinimumWaitTimeServiceMock,
                dateTimeServiceMock,
                filePayloadUploadServiceMock,
                fileDownloadEmailServiceMock);
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadStatsFile();
        }

        private IFileDownloadService NewFileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            ILoggingService loggingService,
            IDownloadService downloadService,
            IFilePayloadSettingsService filePayloadSettingsService,
            IResourceCleanupService resourceCleanupService,
            IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
            IDateTimeService dateTimeService,
            IFilePayloadUploadService filePayloadUploadService,
            IFileDownloadEmailService fileDownloadEmailService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                loggingService,
                downloadService,
                filePayloadSettingsService,
                resourceCleanupService,
                fileDownloadMinimumWaitTimeService,
                dateTimeService,
                filePayloadUploadService,
                fileDownloadEmailService);
        }

        private void SetUpWhenDataStoreIsNotAvailable()
        {
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(false);
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var expected = new Exception();
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw expected; });
            return expected;
        }

        private void SetUpWhenMinimumWaitTimeNotMet()
        {
            fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>()).Returns(false);
        }
    }
}