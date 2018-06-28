namespace StatsDownload.Core.Tests
{
    using System;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestFilePayloadUploadProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload
            {
                DownloadFilePath = DownloadFilePath,
                DecompressedDownloadFilePath = DecompressedDownloadFilePath
            };

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();

            systemUnderTest = NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock,
                fileDownloadDatabaseServiceMock);
        }

        private const string DecompressedDownloadFilePath = "test decompressed download file path";

        private const string DownloadFilePath = "test download file path";

        private IFileCompressionService fileCompressionServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private FilePayload filePayload;

        private IFileReaderService fileReaderServiceMock;

        private IFilePayloadUploadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadUploadProvider(null, fileReaderServiceMock, fileDownloadDatabaseServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadUploadProvider(fileCompressionServiceMock, null, fileDownloadDatabaseServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock, null));
        }

        [Test]
        public void UploadFile_WhenInvoked_UploadsFile()
        {
            InvokeUploadFile();

            Received.InOrder(() =>
            {
                fileCompressionServiceMock.DecompressFile(DownloadFilePath, DecompressedDownloadFilePath);
                fileReaderServiceMock.ReadFile(filePayload);
                fileDownloadDatabaseServiceMock.FileDownloadFinished(filePayload);
            });
        }

        private void InvokeUploadFile()
        {
            systemUnderTest.UploadFile(filePayload);
        }

        private IFilePayloadUploadService NewFilePayloadUploadProvider(IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService,
            IFileDownloadDatabaseService
                fileDownloadDatabaseService)
        {
            return new FilePayloadUploadProvider(fileCompressionService, fileReaderService,
                fileDownloadDatabaseService);
        }
    }
}