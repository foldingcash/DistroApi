namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        private IDownloadService downloadServiceMock;

        private IFileCompressionService fileCompressionServiceMock;

        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFilePayloadSettingsService filePayloadSettingsServiceMock;

        private IFileReaderService fileReaderServiceMock;

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
                    fileCompressionServiceMock,
                    fileReaderServiceMock,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    null,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    null,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    null,
                    fileCompressionServiceMock,
                    fileReaderServiceMock,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    null,
                    fileReaderServiceMock,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    null,
                    resourceCleanupServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    loggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock,
                    null));
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_ReturnsFailedResultWithReason()
        {
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(false);

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_DependenciesCalledInOrder()
        {
            var expected = new Exception();
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw expected; });

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
        public void DownloadFile_WhenExceptionThrown_ExceptionHandled()
        {
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw new Exception(); });

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
            Assert.That(actual.FilePayload, Is.InstanceOf<FilePayload>());
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
                        loggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download started")));
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted(Arg.Any<FilePayload>());
                        filePayloadSettingsServiceMock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>());
                        downloadServiceMock.DownloadFile(Arg.Any<FilePayload>());
                        loggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download completed")));
                        fileCompressionServiceMock.DecompressFile(Arg.Any<FilePayload>());
                        fileReaderServiceMock.ReadFile(Arg.Any<FilePayload>());
                        fileDownloadDataStoreServiceMock.FileDownloadFinished(Arg.Any<FilePayload>());
                        resourceCleanupServiceMock.Cleanup(Arg.Any<FilePayload>());
                        loggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [SetUp]
        public void SetUp()
        {
            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            downloadServiceMock = Substitute.For<IDownloadService>();

            filePayloadSettingsServiceMock = Substitute.For<IFilePayloadSettingsService>();

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            resourceCleanupServiceMock = Substitute.For<IResourceCleanupService>();

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                loggingServiceMock,
                downloadServiceMock,
                filePayloadSettingsServiceMock,
                fileCompressionServiceMock,
                fileReaderServiceMock,
                resourceCleanupServiceMock);
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
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService,
            IResourceCleanupService resourceCleanupService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                loggingService,
                downloadService,
                filePayloadSettingsService,
                fileCompressionService,
                fileReaderService,
                resourceCleanupService);
        }
    }
}