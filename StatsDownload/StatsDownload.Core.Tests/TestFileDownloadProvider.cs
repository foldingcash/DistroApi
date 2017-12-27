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

        private IFileDownloadLoggingService fileDownloadLoggingServiceMock;

        private IFilePayloadSettingsService filePayloadSettingsServiceMock;

        private IFileReaderService fileReaderServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    null,
                    fileDownloadLoggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    null,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    null,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    downloadServiceMock,
                    null,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    null,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    downloadServiceMock,
                    filePayloadSettingsServiceMock,
                    fileCompressionServiceMock,
                    null));
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_ReturnsFailedResultWithReason()
        {
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(false);

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
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
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                        fileDownloadLoggingServiceMock.LogException(expected);
                    }));
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ExceptionHandled()
        {
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw new Exception(); });

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
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
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadDataStoreServiceMock.UpdateToLatest();
                        filePayloadSettingsServiceMock.SetFilePayloadDownloadDetails(Arg.Any<FilePayload>());
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted(Arg.Any<FilePayload>());
                        fileDownloadLoggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download started")));
                        downloadServiceMock.DownloadFile(Arg.Any<FilePayload>());
                        fileDownloadLoggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download completed")));
                        fileCompressionServiceMock.DecompressFile(Arg.Any<FilePayload>());
                        fileReaderServiceMock.ReadFile(Arg.Any<FilePayload>());
                        fileDownloadDataStoreServiceMock.FileDownloadFinished(Arg.Any<FilePayload>());
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [SetUp]
        public void SetUp()
        {
            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(true);

            fileDownloadLoggingServiceMock = Substitute.For<IFileDownloadLoggingService>();

            downloadServiceMock = Substitute.For<IDownloadService>();

            filePayloadSettingsServiceMock = Substitute.For<IFilePayloadSettingsService>();

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                fileDownloadLoggingServiceMock,
                downloadServiceMock,
                filePayloadSettingsServiceMock,
                fileCompressionServiceMock,
                fileReaderServiceMock);
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadStatsFile();
        }

        private IFileDownloadService NewFileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService,
            IDownloadService downloadService,
            IFilePayloadSettingsService filePayloadSettingsService,
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                fileDownloadLoggingService,
                downloadService,
                filePayloadSettingsService,
                fileCompressionService,
                fileReaderService);
        }
    }
}