namespace StatsDownload.Core.Tests
{
    using System;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

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

            loggerMock = Substitute.For<ILogger<ResourceCleanupProvider>>();

            systemUnderTest = NewResourceCleanupProvider(loggerMock, fileServiceMock);
        }

        private FileDownloadResult fileDownloadResult;

        private FilePayload filePayload;

        private IFileService fileServiceMock;

        private ILogger<ResourceCleanupProvider> loggerMock;

        private IResourceCleanupService systemUnderTest;

        [Test]
        public void Cleanup_WhenDecompressedFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DecompressedDownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggerMock.DidNotReceive().LogDebug("Deleting: DecompressedDownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DecompressedDownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenFileDoesNotExist_DoesNotTryDelete()
        {
            fileServiceMock.Exists("DownloadFilePath").Returns(false);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggerMock.DidNotReceive().LogDebug("Deleting: DownloadFilePath");
            fileServiceMock.DidNotReceive().Delete("DownloadFilePath");
        }

        [Test]
        public void CleanUp_WhenFileDownloadFailedDecompression_MovesDownloadFile()
        {
            fileDownloadResult = new FileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);

            systemUnderTest.Cleanup(fileDownloadResult);

            loggerMock.Received().LogDebug("Moving: DownloadFilePath to FailedDownloadFilePath");
            fileServiceMock.Received().Move("DownloadFilePath", "FailedDownloadFilePath");
        }

        [Test]
        public void Cleanup_WhenInvoked_PerformsCleanup()
        {
            systemUnderTest.Cleanup(fileDownloadResult);

            loggerMock.Received().LogDebug("Cleanup Invoked");
            fileServiceMock.Received().Exists("DecompressedDownloadFilePath");
            loggerMock.Received().LogDebug("Deleting: DecompressedDownloadFilePath");
            fileServiceMock.Received().Delete("DecompressedDownloadFilePath");
            fileServiceMock.Received().Exists("DownloadFilePath");
            loggerMock.Received().LogDebug("Deleting: DownloadFilePath");
            fileServiceMock.Received().Delete("DownloadFilePath");
        }

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(null, fileServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewResourceCleanupProvider(loggerMock, null));
        }

        private IResourceCleanupService NewResourceCleanupProvider(ILogger<ResourceCleanupProvider> logger,
                                                                   IFileService fileService)
        {
            return new ResourceCleanupProvider(logger, fileService);
        }
    }
}