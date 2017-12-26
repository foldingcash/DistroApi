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

        private IFileDownloadSettingsService fileDownloadSettingsServiceMock;

        private IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorServiceMock;

        private IFileNameService fileNameServiceMock;

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
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    null,
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    null,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    null,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    null,
                    fileNameServiceMock,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    null,
                    fileCompressionServiceMock,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
                    null,
                    fileReaderServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    downloadServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock,
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
            fileDownloadDataStoreServiceMock.NewFileDownloadStarted().Returns(100);

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.StatsPayload.DownloadId, Is.EqualTo(100));
            Assert.That(actual.StatsPayload.DownloadUrl, Is.EqualTo("DownloadUrl"));
            Assert.That(actual.StatsPayload.TimeoutSeconds, Is.EqualTo(123));
            Assert.That(actual.StatsPayload.DownloadFilePath, Is.EqualTo("DownloadFilePath"));
            Assert.That(actual.StatsPayload.UncompressedDownloadFilePath, Is.EqualTo("UncompressedDownloadFilePath"));
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
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted();
                        fileDownloadLoggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download started")));
                        downloadServiceMock.DownloadFile(
                            Arg.Is<StatsPayload>(
                                payload =>
                                payload.DownloadUrl == "DownloadUrl" && payload.DownloadFilePath == "DownloadFilePath"
                                && payload.TimeoutSeconds == 123));
                        fileDownloadLoggingServiceMock.LogVerbose(
                            Arg.Is<string>(value => value.StartsWith("Stats file download completed")));
                        fileCompressionServiceMock.DecompressFile(Arg.Any<StatsPayload>());
                        fileReaderServiceMock.ReadFile(Arg.Any<StatsPayload>());
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [SetUp]
        public void SetUp()
        {
            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(true);

            fileDownloadLoggingServiceMock = Substitute.For<IFileDownloadLoggingService>();

            fileDownloadSettingsServiceMock = Substitute.For<IFileDownloadSettingsService>();
            fileDownloadSettingsServiceMock.GetDownloadUrl().Returns("DownloadUrl");
            fileDownloadSettingsServiceMock.GetDownloadTimeout().Returns("DownloadTimeoutSeconds");
            fileDownloadSettingsServiceMock.GetDownloadDirectory().Returns("DownloadDirectory");

            downloadServiceMock = Substitute.For<IDownloadService>();

            int timeout;
            fileDownloadTimeoutValidatorServiceMock = Substitute.For<IFileDownloadTimeoutValidatorService>();
            fileDownloadTimeoutValidatorServiceMock.TryParseTimeout("DownloadTimeoutSeconds", out timeout)
                .Returns(
                    callInfo =>
                        {
                            callInfo[1] = 123;
                            return true;
                        });

            fileNameServiceMock = Substitute.For<IFileNameService>();
            fileNameServiceMock.GetFileDownloadPath("DownloadDirectory").Returns("DownloadFilePath");
            fileNameServiceMock.GetUncompressedFileDownloadPath("DownloadDirectory")
                .Returns("UncompressedDownloadFilePath");

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                fileDownloadLoggingServiceMock,
                fileDownloadSettingsServiceMock,
                downloadServiceMock,
                fileDownloadTimeoutValidatorServiceMock,
                fileNameServiceMock,
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
            IFileDownloadSettingsService fileDownloadSettingsService,
            IDownloadService downloadService,
            IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService,
            IFileNameService fileNameService,
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                fileDownloadLoggingService,
                fileDownloadSettingsService,
                downloadService,
                fileDownloadTimeoutValidatorService,
                fileNameService,
                fileCompressionService,
                fileReaderService);
        }
    }
}