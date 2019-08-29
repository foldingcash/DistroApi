namespace StatsDownload.Core.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestFileValidationProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayloadMock = new FilePayload
                              {
                                  DownloadFilePath = "DownloadFilePath",
                                  DecompressedDownloadFilePath = "DecompressedDownloadFilePath"
                              };

            fileCompressionServiceMock = Substitute.For<IFileCompressionService>();

            fileReaderServiceMock = Substitute.For<IFileReaderService>();

            systemUnderTest = new FileValidationProvider(fileCompressionServiceMock, fileReaderServiceMock);
        }

        private IFileCompressionService fileCompressionServiceMock;

        private FilePayload filePayloadMock;

        private IFileReaderService fileReaderServiceMock;

        private IFileValidationService systemUnderTest;

        [Test]
        public void ValidateFile_WhenInvoked_DecompressesAndReadsFile()
        {
            systemUnderTest.ValidateFile(filePayloadMock);

            fileCompressionServiceMock.Received(1).DecompressFile("DownloadFilePath", "DecompressedDownloadFilePath");
            fileReaderServiceMock.Received(1).ReadFile(filePayloadMock);
        }
    }
}