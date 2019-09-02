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

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();

            dataStoreServiceMock = Substitute.For<IDataStoreService>();

            fileValidationServiceMock = Substitute.For<IFileValidationService>();

            dataStoreServiceFactoryMock = Substitute.For<IDataStoreServiceFactory>();
            dataStoreServiceFactoryMock.Create().Returns(dataStoreServiceMock);

            systemUnderTest = NewFilePayloadUploadProvider(fileDownloadDatabaseServiceMock, dataStoreServiceFactoryMock,
                fileValidationServiceMock);
        }

        private const string DecompressedDownloadFilePath = "test decompressed download file path";

        private const string DownloadFilePath = "test download file path";

        private IDataStoreServiceFactory dataStoreServiceFactoryMock;

        private IDataStoreService dataStoreServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private FilePayload filePayload;

        private IFileValidationService fileValidationServiceMock;

        private IFilePayloadUploadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(null, dataStoreServiceFactoryMock, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(fileDownloadDatabaseServiceMock, null, fileValidationServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFilePayloadUploadProvider(fileDownloadDatabaseServiceMock, dataStoreServiceFactoryMock, null));
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

        [TestCase(typeof (FileDownloadFailedDecompressionException))]
        [TestCase(typeof (InvalidStatsFileException))]
        public void UploadFile_WhenValidationExceptionThrown_ExceptionRethrown(Type expectedException)
        {
            var expected = Activator.CreateInstance(expectedException) as Exception;
            fileValidationServiceMock.When(mock => mock.ValidateFile(Arg.Any<FilePayload>())).Throw(expected);

            Assert.Throws(expectedException, InvokeUploadFile);
        }

        private void InvokeUploadFile()
        {
            systemUnderTest.UploadFile(filePayload);
        }

        private IFilePayloadUploadService NewFilePayloadUploadProvider(
            IFileDownloadDatabaseService fileDownloadDatabaseService, IDataStoreServiceFactory dataStoreServiceFactory,
            IFileValidationService fileValidationService)
        {
            return new FilePayloadUploadProvider(fileDownloadDatabaseService, dataStoreServiceFactory,
                fileValidationService);
        }
    }
}