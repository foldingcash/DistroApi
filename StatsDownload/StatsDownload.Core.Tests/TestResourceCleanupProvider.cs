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
        public void Cleanup_WhenInvoked_PerformsCleanup()
        {
            filePayload.DownloadFilePath = "DownloadFilePath";
            filePayload.UncompressedDownloadFilePath = "UncompressedDownloadFilePath";

            systemUnderTest.Cleanup(filePayload);

            Received.InOrder(
                () =>
                    {
                        loggingServiceMock.LogVerbose("Cleanup Invoked");
                        loggingServiceMock.LogVerbose("Deleting: UncompressedDownloadFilePath");
                        fileDeleteServiceMock.Delete("UncompressedDownloadFilePath");
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

            fileDeleteServiceMock = Substitute.For<IFileDeleteService>();

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