namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Logging;

    [TestFixture]
    public class TestResourceCleanupProvider
    {
        [SetUp]
        public void SetUp()
        {
            fileDownloadResult = new FileDownloadResult(filePayload = new FilePayload());
            filePayload.DownloadFilePath = "DownloadFilePath";
            filePayload.DecompressedDownloadFilePath = "DecompressedDownloadFilePath";
            filePayload.FailedDownloadFilePath = "FailedDownloadFilePath";

            fileServiceMock = Substitute.For<IFileService>();
            fileServiceMock.Exists("DecompressedDownloadFilePath").Returns(true);
            fileServiceMock.Exists("DownloadFilePath").Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewResourceCleanupProvider(fileServiceMock, loggingServiceMock);
        }

        private FileDownloadResult fileDownloadResult;

        private FilePayload filePayload;

        private IFileService fileServiceMock;

        private ILoggingService loggingServiceMock;

        private IResourceCleanupService systemUnderTest;

        [Test]
        public void Cleanup_WhenDecompressedFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DecompressedDownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggingServiceMock.DidNotReceive().LogDebug("Deleting: DecompressedDownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DecompressedDownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggingServiceMock.DidNotReceive().LogDebug("Deleting: DownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DownloadFilePath");
        }

        [Test]
        public void CleanUp_WhenFileDownloadFailedDecompression_MovesDownloadFile()
        {
            fileDownloadResult = new FileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggingServiceMock.Received().LogDebug("Moving: DownloadFilePath to FailedDownloadFilePath");
            fileServiceMock.Received().Move("DownloadFilePath", "FailedDownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenInvoked_PerformsCleanup()
        {
            systemUnderTest.Cleanup(fileDownloadResult);

            loggingServiceMock.Received().LogDebug("Cleanup Invoked");
            fileServiceMock.Received().Exists("DecompressedDownloadFilePath");
            loggingServiceMock.Received().LogDebug("Deleting: DecompressedDownloadFilePath");
            fileServiceMock.Received().Delete("DecompressedDownloadFilePath");
            fileServiceMock.Received().Exists("DownloadFilePath");
            loggingServiceMock.Received().LogDebug("Deleting: DownloadFilePath");
            fileServiceMock.Received().Delete("DownloadFilePath");
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(fileServiceMock, null));
        }

        private IResourceCleanupService NewResourceCleanupProvider(IFileService fileService,
                                                                   ILoggingService loggingService)
        {
            return new ResourceCleanupProvider(fileService, loggingService);
        }
    }
}