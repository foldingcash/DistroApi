namespace StatsDownload.Core.Tests
{
    using System;
    using Interfaces.Logging;
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Networking;

    [TestFixture]
    public class TestDownloadProvider
    {
        private IDateTimeService dateTimeServiceMock;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private DownloadProvider systemUnderTest;

        private IWebClientFactory webClientFactoryMock;

        private IWebClient webClientMock;

        [Test]
        public void DownloadFile_WhenInvoked_GetsFromUri()
        {
            systemUnderTest.DownloadFile(filePayload);

            webClientMock.Received(1).DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
        }

        [Test]
        public void DownloadFile_WhenInvoked_SetsTimeout()
        {
            systemUnderTest.DownloadFile(filePayload);

            webClientMock.Received(1).Timeout = TimeSpan.FromSeconds(filePayload.TimeoutSeconds);
        }

        [SetUp]
        public void SetUp()
        {
            loggingServiceMock = Substitute.For<ILoggingService>();
            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            filePayload = GenerateFilePayload();

            webClientMock = Substitute.For<IWebClient>();
            webClientFactoryMock = Substitute.For<IWebClientFactory>();
            webClientFactoryMock.Create().Returns(webClientMock);

            systemUnderTest = new DownloadProvider(loggingServiceMock, dateTimeServiceMock, webClientFactoryMock);
        }

        private FilePayload GenerateFilePayload()
        {
            var payload = new FilePayload();
            payload.TimeoutSeconds = 42;
            payload.DownloadUri = new Uri("https://somesite/somefile.ext");
            payload.DownloadFilePath = "c:\\path\\to\\file.ext";
            payload.AcceptAnySslCert = false;
            return payload;
        }
    }
}