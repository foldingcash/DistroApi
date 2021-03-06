﻿namespace StatsDownload.Core.Tests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Exceptions;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Today;

            loggerMock = Substitute.For<ILogger<FileDownloadProvider>>();

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();
            fileDownloadDatabaseServiceMock.IsAvailable().Returns((true, FailedReason.None));

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

            dataStoreServiceMock = Substitute.For<IDataStoreService>();
            dataStoreServiceMock.IsAvailable().Returns(true);

            dataStoreServiceFactoryMock = Substitute.For<IDataStoreServiceFactory>();
            dataStoreServiceFactoryMock.Create().Returns(dataStoreServiceMock);

            systemUnderTest = NewFileDownloadProvider(loggerMock, fileDownloadDatabaseServiceMock, loggingServiceMock,
                downloadServiceMock, filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock, filePayloadUploadServiceMock,
                fileDownloadEmailServiceMock, dataStoreServiceFactoryMock);
        }

        private IDataStoreServiceFactory dataStoreServiceFactoryMock;

        private IDataStoreService dataStoreServiceMock;

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private IDownloadService downloadServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private IFileDownloadEmailService fileDownloadEmailServiceMock;

        private IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeServiceMock;

        private IFilePayloadSettingsService filePayloadSettingsServiceMock;

        private IFilePayloadUploadService filePayloadUploadServiceMock;

        private ILogger<FileDownloadProvider> loggerMock;

        private IFileDownloadLoggingService loggingServiceMock;

        private IResourceCleanupService resourceCleanupServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(null, fileDownloadDatabaseServiceMock,
                loggingServiceMock, downloadServiceMock, filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock, filePayloadUploadServiceMock,
                fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock, null, loggingServiceMock,
                downloadServiceMock, filePayloadSettingsServiceMock, resourceCleanupServiceMock,
                fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock, filePayloadUploadServiceMock,
                fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, null, downloadServiceMock, filePayloadSettingsServiceMock,
                resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, null, filePayloadSettingsServiceMock,
                resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock, null,
                resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, null, fileDownloadMinimumWaitTimeServiceMock, dateTimeServiceMock,
                filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, resourceCleanupServiceMock, null, dateTimeServiceMock,
                filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock,
                null, filePayloadUploadServiceMock, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock,
                dateTimeServiceMock, null, fileDownloadEmailServiceMock, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock,
                dateTimeServiceMock, filePayloadUploadServiceMock, null, dataStoreServiceFactoryMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(loggerMock,
                fileDownloadDatabaseServiceMock, loggingServiceMock, downloadServiceMock,
                filePayloadSettingsServiceMock, resourceCleanupServiceMock, fileDownloadMinimumWaitTimeServiceMock,
                dateTimeServiceMock, filePayloadUploadServiceMock, fileDownloadEmailServiceMock, null));
        }

        [Test]
        public async Task DownloadFile_WhenDatabaseIsNotAvailable_ReturnsDatabaseUnavailableResult()
        {
            SetUpWhenDatabaseIsNotAvailable();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DatabaseUnavailable));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenDatabaseIsNotAvailable_SendsEmail()
        {
            SetUpWhenDatabaseIsNotAvailable();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public async Task DownloadFile_WhenDataStoreIsNotAvailable_ReturnsDatabaseUnavailableResult()
        {
            SetUpWhenDataStoreIsNotAvailable();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenDataStoreIsNotAvailable_SendsEmail()
        {
            SetUpWhenDataStoreIsNotAvailable();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_LogsException()
        {
            Exception expected = SetUpWhenExceptionThrown();

            InvokeDownloadFile();

            Received.InOrder(() =>
            {
                fileDownloadDatabaseServiceMock.IsAvailable();
                loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                loggerMock.LogError(expected, "There was an unhandled exception");
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
        public async Task DownloadFile_WhenExceptionThrown_ReturnsFailedResultUnexpectedException()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenExceptionThrown_SendsEmail()
        {
            SetUpWhenExceptionThrown();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadConnectFailure_ReturnsFailedResultFileDownloadNotFound()
        {
            SetUpFileDownloadConnectFailure();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.FileDownloadNotFound));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadFailedDecompressions_ReturnsFileDownloadFailedDecompression()
        {
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable())
                                           .Do(info => throw new FileDownloadFailedDecompressionException());

            FileDownloadResult actual = await InvokeDownloadFile();

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
                loggerMock.LogError(exception, "There was an unhandled exception");
            });
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadFails_ResourceCleanupInvoked()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = await InvokeDownloadFile();

            resourceCleanupServiceMock.Received().Cleanup(actual);
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadFails_ReturnsFailedResultUnexpectedException()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadFails_SendsEmail()
        {
            SetUpFileDownloadFails();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadProtocolError_ReturnsFailedResultFileDownloadNotFound()
        {
            SetUpFileDownloadProtocolError();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.FileDownloadNotFound));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadSettingsInvalid_ReturnsFailedReasonRequiredSettingsInvalid()
        {
            SetUpFileDownloadSettingsInvalid();

            FileDownloadResult actual = await InvokeDownloadFile();

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
                loggerMock.LogError(exception, "There was an unhandled exception");
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
        public async Task DownloadFile_WhenFileDownloadTimeout_ReturnsFailedResultFileDownloadTimeout()
        {
            SetUpFileDownloadTimeout();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.FileDownloadTimeout));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenFileDownloadTimeout_SendsEmail()
        {
            SetUpFileDownloadTimeout();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [TestCase(typeof (FileDownloadFailedDecompressionException))]
        [TestCase(typeof (InvalidStatsFileException))]
        [TestCase(typeof (UnexpectedValidationException))]
        public void DownloadFile_WhenFileValidationFails_UpdatesToFileValidationError(Type exceptionType)
        {
            SetUpFileValidationError(exceptionType);

            InvokeDownloadFile();

            fileDownloadDatabaseServiceMock.Received().FileValidationError(Arg.Any<FileDownloadResult>());
        }

        [Test]
        public async Task DownloadFile_WhenInvoked_ResultIsSuccessAndContainsDownloadData()
        {
            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenInvoked_StatsDownloadIsPerformed()
        {
            InvokeDownloadFile();

            Received.InOrder(async () =>
            {
                fileDownloadDatabaseServiceMock.IsAvailable();
                fileDownloadDatabaseServiceMock.UpdateToLatest();
                dateTimeServiceMock.DateTimeNow();
                loggerMock.LogDebug($"Stats file download started: {dateTime}");
                fileDownloadDatabaseServiceMock.FileDownloadStarted(Arg.Any<FilePayload>());
                fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>());
                filePayloadSettingsServiceMock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>());
                downloadServiceMock.DownloadFile(Arg.Any<FilePayload>());
                dateTimeServiceMock.DateTimeNow();
                loggerMock.LogDebug($"Stats file download completed: {dateTime}");
                await filePayloadUploadServiceMock.UploadFile(Arg.Any<FilePayload>());
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
        public async Task DownloadFile_WhenMinimumWaitTimeNotMet_ReturnsFailedResultMinimumWaitTimeNotMet()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            FileDownloadResult actual = await InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.MinimumWaitTimeNotMet));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public async Task DownloadFile_WhenMinimumWaitTimeNotMet_SendsEmail()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            FileDownloadResult actual = await InvokeDownloadFile();

            fileDownloadEmailServiceMock.Received().SendEmail(actual);
        }

        [Test]
        public void DownloadFile_WhenMinimumWaitTimeNotMet_UpdatesDownloadToError()
        {
            SetUpWhenMinimumWaitTimeNotMet();

            InvokeDownloadFile();

            fileDownloadDatabaseServiceMock.Received().FileDownloadError(Arg.Any<FileDownloadResult>());
        }

        private Task<FileDownloadResult> InvokeDownloadFile()
        {
            return systemUnderTest.DownloadStatsFile();
        }

        private IFileDownloadService NewFileDownloadProvider(ILogger<FileDownloadProvider> logger,
                                                             IFileDownloadDatabaseService fileDownloadDatabaseService,
                                                             IFileDownloadLoggingService loggingService,
                                                             IDownloadService downloadService,
                                                             IFilePayloadSettingsService filePayloadSettingsService,
                                                             IResourceCleanupService resourceCleanupService,
                                                             IFileDownloadMinimumWaitTimeService
                                                                 fileDownloadMinimumWaitTimeService,
                                                             IDateTimeService dateTimeService,
                                                             IFilePayloadUploadService filePayloadUploadService,
                                                             IFileDownloadEmailService fileDownloadEmailService,
                                                             IDataStoreServiceFactory dataStoreServiceFactory)
        {
            return new FileDownloadProvider(logger, fileDownloadDatabaseService, loggingService, downloadService,
                filePayloadSettingsService, resourceCleanupService, fileDownloadMinimumWaitTimeService, dateTimeService,
                filePayloadUploadService, fileDownloadEmailService, dataStoreServiceFactory);
        }

        private void SetUpFileDownloadConnectFailure()
        {
            SetUpFileDownloadWebException(WebExceptionStatus.ConnectFailure);
        }

        private WebException SetUpFileDownloadFails()
        {
            var exception = new WebException();
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => throw exception);
            return exception;
        }

        private void SetUpFileDownloadProtocolError()
        {
            SetUpFileDownloadWebException(WebExceptionStatus.ProtocolError);
        }

        private void SetUpFileDownloadSettingsInvalid()
        {
            filePayloadSettingsServiceMock.When(mock => mock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>()))
                                          .Throw(new FileDownloadArgumentException());
        }

        private WebException SetUpFileDownloadTimeout()
        {
            return SetUpFileDownloadWebException(WebExceptionStatus.Timeout);
        }

        private WebException SetUpFileDownloadWebException(WebExceptionStatus webExceptionStatus)
        {
            var exception = new WebException("sampleWebException", webExceptionStatus);
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => throw exception);
            return exception;
        }

        private void SetUpFileValidationError(Type exceptionType)
        {
            filePayloadUploadServiceMock.When(mock => mock.UploadFile(Arg.Any<FilePayload>()))
                                        .Throw(Activator.CreateInstance(exceptionType) as Exception);
        }

        private void SetUpWhenDatabaseIsNotAvailable()
        {
            fileDownloadDatabaseServiceMock.IsAvailable().Returns((false, FailedReason.DatabaseUnavailable));
        }

        private void SetUpWhenDataStoreIsNotAvailable()
        {
            dataStoreServiceMock.IsAvailable().Returns(false);
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var expected = new Exception();
            fileDownloadDatabaseServiceMock.When(mock => mock.IsAvailable()).Do(info => throw expected);
            return expected;
        }

        private void SetUpWhenMinimumWaitTimeNotMet()
        {
            fileDownloadMinimumWaitTimeServiceMock.IsMinimumWaitTimeMet(Arg.Any<FilePayload>()).Returns(false);
        }
    }
}