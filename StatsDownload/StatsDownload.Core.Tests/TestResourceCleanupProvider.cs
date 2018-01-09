namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestResourceCleanupProvider
    {
        private IFileDeleteService fileDeleteServiceMock;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private IResourceCleanupService systemUnderTest;

        [Test]
        public void Cleanup_WhenFileDoesNotExist_DoesNotTryDelete()
        {
            fileDeleteServiceMock.Exists("DownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(filePayload);

            loggingServiceMock.DidNotReceive().LogVerbose("Deleting: DownloadFilePath");
            fileDeleteServiceMock.DidNotReceive().Delete("DownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenInvoked_PerformsCleanup()
        {
            systemUnderTest.Cleanup(filePayload);

            Received.InOrder(
                () =>
                    {
                        loggingServiceMock.LogVerbose("Cleanup Invoked");
                        fileDeleteServiceMock.Exists("UncompressedDownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: UncompressedDownloadFilePath");
                        fileDeleteServiceMock.Delete("UncompressedDownloadFilePath");
                        fileDeleteServiceMock.Exists("DownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: DownloadFilePath");
                        fileDeleteServiceMock.Delete("DownloadFilePath");
                    });
        }

        [Test]
        public void Cleanup_WhenUncompressedFileDoesNotExist_DoesNotTryDelete()
        {
            fileDeleteServiceMock.Exists("UncompressedDownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(filePayload);

            loggingServiceMock.DidNotReceive().LogVerbose("Deleting: UncompressedDownloadFilePath");
            fileDeleteServiceMock.DidNotReceive().Delete("UncompressedDownloadFilePath");
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(fileDeleteServiceMock, null));
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();
            filePayload.DownloadFilePath = "DownloadFilePath";
            filePayload.UncompressedDownloadFilePath = "UncompressedDownloadFilePath";

            fileDeleteServiceMock = Substitute.For<IFileDeleteService>();
            fileDeleteServiceMock.Exists("UncompressedDownloadFilePath").Returns(true);
            fileDeleteServiceMock.Exists("DownloadFilePath").Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewResourceCleanupProvider(fileDeleteServiceMock, loggingServiceMock);
        }

        private IResourceCleanupService NewResourceCleanupProvider(
            IFileDeleteService fileDeleteService,
            ILoggingService loggingService)
        {
            return new ResourceCleanupProvider(fileDeleteService, loggingService);
        }
    }
}