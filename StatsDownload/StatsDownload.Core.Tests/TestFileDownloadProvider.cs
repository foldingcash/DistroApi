namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFileDownloadLoggingService fileDownloadLoggingServiceMock;

        private IFileDownloadSettingsService fileDownloadSettingsServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => NewFileDownloadProvider(null, fileDownloadLoggingServiceMock, fileDownloadSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFileDownloadProvider(fileDownloadDataStoreServiceMock, null, fileDownloadSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFileDownloadProvider(fileDownloadDataStoreServiceMock, fileDownloadLoggingServiceMock, null));
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
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadFile Invoked");
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
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadDataStoreServiceMock.UpdateToLatest();
                        fileDownloadDataStoreServiceMock.NewFileDownloadStarted();
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
            Assert.That(actual.DownloadDirectory, Is.EqualTo("DownloadDirectory"));
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

            systemUnderTest = NewFileDownloadProvider(
                fileDownloadDataStoreServiceMock,
                fileDownloadLoggingServiceMock,
                fileDownloadSettingsServiceMock);
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadFile();
        }

        private IFileDownloadService NewFileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService,
            IFileDownloadSettingsService fileDownloadSettingsService)
        {
            return new FileDownloadProvider(
                fileDownloadDataStoreService,
                fileDownloadLoggingService,
                fileDownloadSettingsService);
        }
    }
}