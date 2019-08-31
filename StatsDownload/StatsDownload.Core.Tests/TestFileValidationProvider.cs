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

            statsFileParserServiceMock = Substitute.For<IStatsFileParserService>();

            systemUnderTest = new FileValidationProvider(fileCompressionServiceMock, fileReaderServiceMock,
                statsFileParserServiceMock);
        }

        private IFileCompressionService fileCompressionServiceMock;

        private FilePayload filePayloadMock;

        private IFileReaderService fileReaderServiceMock;

        private IStatsFileParserService statsFileParserServiceMock;

        private IFileValidationService systemUnderTest;

        [Test]
        public void ValidateFile_WhenInvoked_ValidatesFile()
        {
            systemUnderTest.ValidateFile(filePayloadMock);

            Received.InOrder(() =>
            {
                fileCompressionServiceMock
                    .Received(1).DecompressFile("DownloadFilePath", "DecompressedDownloadFilePath");
                fileReaderServiceMock.Received(1).ReadFile(filePayloadMock);
                statsFileParserServiceMock.Parse(filePayloadMock);
            });
        }
    }
}