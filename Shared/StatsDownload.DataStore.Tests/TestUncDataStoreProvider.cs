namespace StatsDownload.DataStore.Tests
{
    using System;
    using System.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestUncDataStoreProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayloadMock = new FilePayload
                              {
                                  DownloadFilePath = "\\DownloadDirectory\\Source.ext",
                                  UploadPath = "\\UploadDirectory\\Target.ext"
                              };

            validatedFileMock = new ValidatedFile(0, DateTime.UtcNow, "\\ValidatedFilePath\\Source.ext");

            dataStoreSettingsMock = Substitute.For<IDataStoreSettings>();

            dataStoreSettingsMock.UploadDirectory.Returns("C:\\Path");

            directoryServiceMock = Substitute.For<IDirectoryService>();

            fileServiceMock = Substitute.For<IFileService>();

            systemUnderTest = new UncDataStoreProvider(dataStoreSettingsMock, directoryServiceMock, fileServiceMock);
        }

        private IDataStoreSettings dataStoreSettingsMock;

        private IDirectoryService directoryServiceMock;

        private FilePayload filePayloadMock;

        private IFileService fileServiceMock;

        private IDataStoreService systemUnderTest;

        private ValidatedFile validatedFileMock;

        [Test]
        public async Task DownloadFile_WhenInvoked_CopysFile()
        {
            await systemUnderTest.DownloadFile(filePayloadMock, validatedFileMock);

            fileServiceMock.Received(1).CopyFile("\\ValidatedFilePath\\Source.ext", "\\UploadDirectory\\Target.ext");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task IsAvailable_WhenInvoked_CheckForAccessToUploadDirectory(bool expected)
        {
            directoryServiceMock.Exists("C:\\Path").Returns(expected);

            bool actual = await systemUnderTest.IsAvailable();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task UploadFile_WhenInvoked_CopysDownloadFile()
        {
            await systemUnderTest.UploadFile(filePayloadMock);

            fileServiceMock.Received(1).CopyFile("\\DownloadDirectory\\Source.ext", "\\UploadDirectory\\Target.ext");
        }
    }
}