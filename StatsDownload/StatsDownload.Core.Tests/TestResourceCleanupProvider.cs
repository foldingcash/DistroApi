namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Logging;

    [TestFixture]
    public class TestResourceCleanupProvider
    {
        private IFileDeleteService fileDeleteServiceMock;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private IResourceCleanupService systemUnderTest;

        [Test]
        public void Cleanup_WhenDecompressedFileDoesNotExist_DoesNotTryDelete()
        {
            fileDeleteServiceMock.Exists("DencompressedDownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(filePayload);

            loggingServiceMock.DidNotReceive().LogVerbose("Deleting: DencompressedDownloadFilePath");
            fileDeleteServiceMock.DidNotReceive().Delete("DencompressedDownloadFilePath");
        }

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
                        fileDeleteServiceMock.Exists("DencompressedDownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: DencompressedDownloadFilePath");
                        fileDeleteServiceMock.Delete("DencompressedDownloadFilePath");
                        fileDeleteServiceMock.Exists("DownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: DownloadFilePath");
                        fileDeleteServiceMock.Delete("DownloadFilePath");
                    });
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
            filePayload.DecompressedDownloadFilePath = "DencompressedDownloadFilePath";

            fileDeleteServiceMock = Substitute.For<IFileDeleteService>();
            fileDeleteServiceMock.Exists("DencompressedDownloadFilePath").Returns(true);
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