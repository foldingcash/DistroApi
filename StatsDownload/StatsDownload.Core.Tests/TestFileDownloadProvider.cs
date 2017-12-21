namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFileDownloaderService fileDownloaderServiceMock;

        private IFileDownloadLoggingService fileDownloadLoggingServiceMock;

        private IFileDownloadSettingsService fileDownloadSettingsServiceMock;

        private IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorServiceMock;

        private IFileNameService fileNameServiceMock;

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
                    fileDownloaderServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    null,
                    fileDownloadSettingsServiceMock,
                    fileDownloaderServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    null,
                    fileDownloaderServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    null,
                    fileDownloadTimeoutValidatorServiceMock,
                    fileNameServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    fileDownloaderServiceMock,
                    null,
                    fileNameServiceMock));
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFileDownloadProvider(
                    fileDownloadDataStoreServiceMock,
                    fileDownloadLoggingServiceMock,
                    fileDownloadSettingsServiceMock,
                    fileDownloaderServiceMock,
                    fileDownloadTimeoutValidatorServiceMock,
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
        public void DownloadFile_WhenInvoked_DependenciesCalledInOrder()
        {
            InvokeDownloadFile();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadStatsFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadDataStoreServiceMock.UpdateToLatest();
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted();
                        fileDownloaderServiceMock.DownloadFile("DownloadUrl", "DownloadFileName", 123);
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [Test]
        public void DownloadFile_WhenInvoked_ResultIsSuccessAndContainsDownloadData()
        {
            fileDownloadDataStoreServiceMock.NewFileDownloadStarted().Returns(100);

            FileDownloadResult actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.DownloadId, Is.EqualTo(100));
            Assert.That(actual.DownloadUrl, Is.EqualTo("DownloadUrl"));
            Assert.That(actual.DownloadTimeoutSeconds, Is.EqualTo("DownloadTimeoutSeconds"));
            Assert.That(actual.DownloadFileName, Is.EqualTo("DownloadFileName"));
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

            fileDownloaderServiceMock = Substitute.For<IFileDownloaderService>();

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
            fileNameServiceMock.GetRandomFileNamePath("DownloadDirectory").Returns("DownloadFileName");

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                fileDownloadLoggingServiceMock,
                fileDownloadSettingsServiceMock,
                fileDownloaderServiceMock,
                fileDownloadTimeoutValidatorServiceMock,
                fileNameServiceMock);
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadStatsFile();
        }

        private IFileDownloadService NewFileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService,
            IFileDownloadSettingsService fileDownloadSettingsService,
            IFileDownloaderService fileDownloaderService,
            IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService,
            IFileNameService fileNameService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                fileDownloadLoggingService,
                fileDownloadSettingsService,
                fileDownloaderService,
                fileDownloadTimeoutValidatorService,
                fileNameService);
        }
    }
}