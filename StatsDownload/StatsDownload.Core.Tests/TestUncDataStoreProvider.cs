namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    [TestFixture]
    public class TestUncDataStoreProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayloadMock = new FilePayload
                              {
                                  DownloadFilePath = "\\DownloadDirectory\\Source.ext",
                                  UploadPath = "\\UncUploadDirectory\\Target.ext"
                              };

            uncDataStoreSettingsMock = Substitute.For<IUncDataStoreSettings>();

            uncDataStoreSettingsMock.UncUploadDirectory.Returns(new Uri("C:\\Path"));

            directoryServiceMock = Substitute.For<IDirectoryService>();

            fileServiceMock = Substitute.For<IFileService>();

            systemUnderTest = new UncDataStoreProvider(uncDataStoreSettingsMock, directoryServiceMock, fileServiceMock);
        }

        private IDirectoryService directoryServiceMock;

        private FilePayload filePayloadMock;

        private IFileService fileServiceMock;

        private IDataStoreService systemUnderTest;

        private IUncDataStoreSettings uncDataStoreSettingsMock;

        [Test]
        public void IsAvailable_WhenDirectoryNotAvailable_ReturnsDataStoreUnavailable()
        {
            directoryServiceMock.Exists("C:\\Path").Returns(false);

            (bool, FailedReason failedReason) actual = systemUnderTest.IsAvailable();

            Assert.That(actual.failedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsAvailable_WhenInvoked_CheckForAccessToUploadDirectory(bool expected)
        {
            directoryServiceMock.Exists("C:\\Path").Returns(expected);

            (bool isAvailable, FailedReason) actual = systemUnderTest.IsAvailable();

            Assert.That(actual.isAvailable, Is.EqualTo(expected));
        }

        [Test]
        public void UploadFile_WhenInvoked_CopysDownloadFile()
        {
            systemUnderTest.UploadFile(filePayloadMock);

            fileServiceMock.Received(1).CopyFile("\\DownloadDirectory\\Source.ext", "\\UncUploadDirectory\\Target.ext");
        }
    }
}