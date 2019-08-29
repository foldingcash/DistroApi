namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Exceptions;
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

            fileValidationServiceMock = Substitute.For<IFileValidationService>();

            systemUnderTest = NewFilePayloadUploadProvider(fileCompressionServiceMock, fileReaderServiceMock,
                fileDownloadDatabaseServiceMock, dataStoreServiceMock, fileValidationServiceMock);
        }

        private const string DecompressedDownloadFilePath = "test decompressed download file path";

        private const string DownloadFilePath = "test download file path";

        private IDataStoreService dataStoreServiceMock;

        private IFileCompressionService fileCompressionServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private FilePayload filePayload;

        private IFileReaderService fileReaderServiceMock;

        private IFileValidationService fileValidationServiceMock;

        private IFilePayloadUploadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadUploadProvider(null, fileReaderServiceMock,
                fileDownloadDatabaseServiceMock, dataStoreServiceMock, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadUploadProvider(fileCompressionServiceMock, null,
                fileDownloadDatabaseServiceMock, dataStoreServiceMock, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadUploadProvider(fileCompressionServiceMock,
                fileReaderServiceMock, null, dataStoreServiceMock, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadUploadProvider(fileCompressionServiceMock,
                fileReaderServiceMock, fileDownloadDatabaseServiceMock, null, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadUploadProvider(fileCompressionServiceMock,
                fileReaderServiceMock, fileDownloadDatabaseServiceMock, dataStoreServiceMock, null));
        }

        [Test]
        public void UploadFile_WhenInvoked_UploadsFile()
        {
            InvokeUploadFile();

            Received.InOrder(() =>
            {
                dataStoreServiceMock.UploadFile(filePayload);
                fileDownloadDatabaseServiceMock.FileDownloadFinished(filePayload);
                fileDownloadDatabaseServiceMock.FileValidationStarted(filePayload);
                fileValidationServiceMock.ValidateFile(filePayload);
                fileDownloadDatabaseServiceMock.FileValidated(filePayload);
            });
        }

        [Test]
        public void
            UploadFile_WhenUnexpectedValidationExceptionThrown_ThrownUnexpectedValidationExceptionWithInnerException()
        {
            var expected = new Exception();
            fileValidationServiceMock.When(mock => mock.ValidateFile(Arg.Any<FilePayload>())).Throw(expected);

            var actual = Assert.Throws<UnexpectedValidationException>(InvokeUploadFile);
            Assert.That(actual.InnerException, Is.EqualTo(expected));
        }

        private void InvokeUploadFile()
        {
            systemUnderTest.UploadFile(filePayload);
        }

        private IFilePayloadUploadService NewFilePayloadUploadProvider(IFileCompressionService fileCompressionService,
                                                                       IFileReaderService fileReaderService,
                                                                       IFileDownloadDatabaseService
                                                                           fileDownloadDatabaseService,
                                                                       IDataStoreService dataStoreService,
                                                                       IFileValidationService fileValidationService)
        {
            return new FilePayloadUploadProvider(fileCompressionService, fileReaderService, fileDownloadDatabaseService,
                dataStoreService, fileValidationService);
        }
    }
}