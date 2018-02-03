namespace StatsDownload.Core.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFilePayloadUploadProvider
    {
        private IFileCompressionService fileCompressionServiceMock;

        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private FilePayload filePayload;

        private IFileReaderService fileReaderServiceMock;

        private IFilePayloadUploadService systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();

            systemUnderTest = new FilePayloadUploadProvider(
                fileCompressionServiceMock,
                fileReaderServiceMock,
                fileDownloadDataStoreServiceMock);
        }

        [Test]
        public void UploadFile_WhenInvoked_UploadsFile()
        {
            InvokeUploadFile();

            Received.InOrder(
                () =>
                    {
                        fileCompressionServiceMock.DecompressFile(filePayload);
                        fileReaderServiceMock.ReadFile(filePayload);
                        fileDownloadDataStoreServiceMock.FileDownloadFinished(filePayload);
                    });
        }

        private void InvokeUploadFile()
        {
            systemUnderTest.UploadFile(filePayload);
        }
    }
}