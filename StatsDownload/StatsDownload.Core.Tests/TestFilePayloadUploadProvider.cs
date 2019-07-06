namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

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

            dataStoreServiceMock = Substitute.For<IDataStoreService>();

            systemUnderTest = NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock,
                fileDownloadDatabaseServiceMock, dataStoreServiceMock);
        }

        private IDataStoreService dataStoreServiceMock;

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
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(null, fileReaderServiceMock, fileDownloadDatabaseServiceMock, dataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(fileCompressionServiceMock, null, fileDownloadDatabaseServiceMock, dataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock, null, dataStoreServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock, fileDownloadDatabaseServiceMock, null));
        }

        [Test]
        public void UploadFile_WhenInvoked_UploadsFile()
        {
            InvokeUploadFile();

            Received.InOrder(() =>
            {
                fileCompressionServiceMock.DecompressFile(DownloadFilePath, DecompressedDownloadFilePath);
                fileReaderServiceMock.ReadFile(filePayload);
                dataStoreServiceMock.UploadFile(filePayload);
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
                                                                           fileDownloadDatabaseService,
                                                                       IDataStoreService dataStoreService)
        {
            return new FilePayloadUploadProvider(fileCompressionService, fileReaderService,
                fileDownloadDatabaseService, dataStoreService);
        }
    }
}