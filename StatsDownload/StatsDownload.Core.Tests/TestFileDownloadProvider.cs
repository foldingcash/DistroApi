namespace StatsDownload.Core.Tests
{
    using System;
    using System.Net;
    using Exceptions;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Today;

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();
            fileDownloadDatabaseServiceMock.IsAvailable().Returns(true);

            loggingServiceMock = Substitute.For<IFileDownloadLoggingService>();

            downloadServiceMock = Substitute.For<IDownloadService>();

            filePayloadSettingsServiceMock = Substitute.For<IFilePayloadSettingsService>();

            resourceCleanupServiceMock = Substitute.For<IResourceCleanupService>();

            fileDownloadMinimumWaitTimeServiceMock = Substitute.For<IFileDownloadMinimumWaitTimeService>();
            fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>()).Returns(true);

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            filePayloadUploadServiceMock = Substitute.For<IFilePayloadUploadService>();

            fileDownloadEmailServiceMock = Substitute.For<IFileDownloadEmailService>();

            systemUnderTest = NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock,
                downloadServiceMock, filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock, filePayloadUploadServiceMock,
                fileDownloadEmailServiceMock);
        }

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private IDownloadService downloadServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private IFileDownloadEmailService fileDownloadEmailServiceMock;

        private IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeServiceMock;

        private IFilePayloadSettingsService filePayloadSettingsServiceMock;

        private IFilePayloadUploadService filePayloadUploadServiceMock;

        private IFileDownloadLoggingService loggingServiceMock;

        private IResourceCleanupService resourceCleanupServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(null, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock,
                        resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                        filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, null, downloadServiceMock,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                        fileDownloadMinimumWaitTimeServiceMock,
                        dateTimeServiceMock, filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, null,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                        fileDownloadMinimumWaitTimeServiceMock,
                        dateTimeServiceMock, filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        null,
                        resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                        filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock, null, fileDownloadMinimumWaitTimeServiceMock,
                        dateTimeServiceMock,
                        filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock, null, dateTimeServiceMock,
                        filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                        fileDownloadMinimumWaitTimeServiceMock,
                        null, filePayloadUploadServiceMock, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                        fileDownloadMinimumWaitTimeServiceMock,
                        dateTimeServiceMock, null, fileDownloadEmailServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                    NewFileDownloadProvider(fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                        filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                        fileDownloadMinimumWaitTimeServiceMock,
                        dateTimeServiceMock, filePayloadUploadServiceMock, null));
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_ReturnsDataStoreUnavailableResult()
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
        public void DownloadFile_WhenExceptionThrown_LogsException()
        {
            Exception expected = SetUpWhenExceptionThrown();

            InvokeDownloadFile();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                fileDownloadDatabaseServiceMock.IsAvailable();
                loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                loggingServiceMock.LogException(expected);
            });
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ResourceCleanupInvoked()
        {
            SetUpWhenExceptionThrown();

            InvokeDownloadFile();

            resourceCleanupServiceMock.Received().Cleanup(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ReturnsFailedResultUnexpectedException()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_SendsEmail()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenFileDownloadFailedDecompressions_ReturnsFileDownloadFailedDecompression()
        {
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable())
                                           .Do(
                                               info =>
                                               {
                                                   throw new FileDownloadFailedDecompressionException(string.Empty);
                                               });

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.FileDownloadFailedDecompression));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadFails_LogsException()
        {
            WebException exception = SetUpFileDownloadFails();

            InvokeDownloadFile();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                loggingServiceMock.LogException(exception);
            });
        }

        [Test]
        public void DownloadFile_WhenFileDownloadFails_ResourceCleanupInvoked()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = InvokeDownloadFile();

            resourceCleanupServiceMock.Received().Cleanup(actual);
        }

        [Test]
        public void DownloadFile_WhenFileDownloadFails_ReturnsFailedResultUnexpectedException()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadFails_SendsEmail()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenFileDownloadSettingsInvalid_ReturnsFailedReasonRequiredSettingsInvalid()
        {
            SetUpFileDownloadSettingsInvalid();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.RequiredSettingsInvalid));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadSettingsInvalid_UpdatesDownloadToError()
        {
            SetUpFileDownloadSettingsInvalid();

            InvokeDownloadFile();

            fileDownloadDatabaseServiceMock.Received().FileDownloadError(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadTimeout_LogsException()
        {
            WebException exception = SetUpFileDownloadTimeout();

            InvokeDownloadFile();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                loggingServiceMock.LogException(exception);
            });
        }

        [Test]
        public void DownloadFile_WhenFileDownloadTimeout_ResourceCleanupInvoked()
        {
            SetUpFileDownloadTimeout();

            InvokeDownloadFile();

            resourceCleanupServiceMock.Received().Cleanup(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadTimeout_ReturnsFailedResultFileDownloadTimeout()
        {
            SetUpFileDownloadTimeout();

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.FileDownloadTimeout));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenFileDownloadTimeout_SendsEmail()
        {
            SetUpFileDownloadTimeout();

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

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                fileDownloadDatabaseServiceMock.IsAvailable();
                fileDownloadDatabaseServiceMock.UpdateToLatest();
                dateTimeServiceMock.DateTimeNow();
                loggingServiceMock.LogVerbose($"Stats file download started: {dateTime}");
                fileDownloadDatabaseServiceMock.NewFileDownloadStarted(Arg.Any<FilePayload>());
                fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>());
                filePayloadSettingsServiceMock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>());
                downloadServiceMock.DownloadFile(Arg.Any<FilePayload>());
                dateTimeServiceMock.DateTimeNow();
                loggingServiceMock.LogVerbose($"Stats file download completed: {dateTime}");
                filePayloadUploadServiceMock.UploadFile(Arg.Any<FilePayload>());
                resourceCleanupServiceMock.Cleanup(Arg.Any<FileDownloadResult>());
                loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
            });
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_LogsDownloadResult()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            InvokeDownloadFile();

            loggingServiceMock.Received().LogResult(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_ReturnsFailedResultMinimumWaitTimeNotMet()
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

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_UpdatesDownloadToError()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            InvokeDownloadFile();

            fileDownloadDatabaseServiceMock.Received().FileDownloadError(Arg.Any<FileDownloadResult>());
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadStatsFile();
        }

        private IFileDownloadService NewFileDownloadProvider(IFileDownloadDatabaseService fileDownloadDatabaseService,
            IFileDownloadLoggingService loggingService,
            IDownloadService downloadService,
            IFilePayloadSettingsService filePayloadSettingsService,
            IResourceCleanupService resourceCleanupService,
            IFileDownloadMinimumWaitTimeService
                fileDownloadMinimumWaitTimeService,
            IDateTimeService dateTimeService,
            IFilePayloadUploadService filePayloadUploadService,
            IFileDownloadEmailService fileDownloadEmailService)
        {
            return new FileDownloadProvider(fileDownloadDatabaseService, loggingService, downloadService,
                filePayloadSettingsService, resourceCleanupService, fileDownloadMinimumWaitTimeService, dateTimeService,
                filePayloadUploadService, fileDownloadEmailService);
        }

        private WebException SetUpFileDownloadFails()
        {
            var exception = new WebException();
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw exception; });
            return exception;
        }

        private void SetUpFileDownloadSettingsInvalid()
        {
            filePayloadSettingsServiceMock.When(mock => mock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>()))
                                          .Throw(new FileDownloadArgumentException(string.Empty));
        }

        private WebException SetUpFileDownloadTimeout()
        {
            var exception = new WebException("timeout", WebExceptionStatus.Timeout);
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw exception; });
            return exception;
        }

        private void SetUpWhenDataStoreIsNotAvailable()
        {
            fileDownloadDatabaseServiceMock.IsAvailable().Returns(false);
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var expected = new Exception();
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw expected; });
            return expected;
        }

        private void SetUpWhenMinimumWaitTimeNotMet()
        {
            fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>()).Returns(false);
        }
    }
}