namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Logging;

    [TestFixture]
    public class TestResourceCleanupProvider
    {
        private FilePayload filePayload;

        private IFileService fileServiceMock;

        private ILoggingService loggingServiceMock;

        private IResourceCleanupService systemUnderTest;

        [Test]
        public void Cleanup_WhenDecompressedFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DencompressedDownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(filePayload);

            loggingServiceMock.DidNotReceive().LogVerbose("Deleting: DencompressedDownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DencompressedDownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(filePayload);

            loggingServiceMock.DidNotReceive().LogVerbose("Deleting: DownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenInvoked_PerformsCleanup()
        {
            systemUnderTest.Cleanup(filePayload);

            Received.InOrder(
                () =>
                    {
                        loggingServiceMock.LogVerbose("Cleanup Invoked");
                        fileServiceMock.Exists("DencompressedDownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: DencompressedDownloadFilePath");
                        fileServiceMock.Delete("DencompressedDownloadFilePath");
                        fileServiceMock.Exists("DownloadFilePath");
                        loggingServiceMock.LogVerbose("Deleting: DownloadFilePath");
                        fileServiceMock.Delete("DownloadFilePath");
                    });
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(fileServiceMock, null));
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();
            filePayload.DownloadFilePath = "DownloadFilePath";
            filePayload.DecompressedDownloadFilePath = "DencompressedDownloadFilePath";

            fileServiceMock = Substitute.For<IFileService>();
            fileServiceMock.Exists("DencompressedDownloadFilePath").Returns(true);
            fileServiceMock.Exists("DownloadFilePath").Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewResourceCleanupProvider(fileServiceMock, loggingServiceMock);
        }

        private IResourceCleanupService NewResourceCleanupProvider(
            IFileService fileService,
            ILoggingService loggingService)
        {
            return new ResourceCleanupProvider(fileService, loggingService);
        }
    }
}