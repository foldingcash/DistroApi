namespace StatsDownload.Core.Tests
{
    using System;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Networking;

    [TestFixture]
    public class TestDownloadProvider
    {
        [SetUp]
        public void SetUp()
        {
            loggerMock = Substitute.For<ILogger<DownloadProvider>>();

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            filePayload = GenerateFilePayload();

            webClientMock = Substitute.For<IWebClient>();
            webClientFactoryMock = Substitute.For<IWebClientFactory>();
            webClientFactoryMock.Create().Returns(webClientMock);

            systemUnderTest = new DownloadProvider(loggerMock, dateTimeServiceMock, webClientFactoryMock);
        }

        private IDateTimeService dateTimeServiceMock;

        private FilePayload filePayload;

        private ILogger<DownloadProvider> loggerMock;

        private DownloadProvider systemUnderTest;

        private IWebClientFactory webClientFactoryMock;

        private IWebClient webClientMock;

        [Test]
        public void DownloadFile_WhenErrorOccursDuringDownload_ReleasesWebClient()
        {
            webClientMock.When(client => client.DownloadFile(Arg.Any<Uri>(), Arg.Any<string>()))
                         .Do(callInfo => { throw new Exception(); });

            Assert.Throws<Exception>(() => systemUnderTest.DownloadFile(filePayload));

            Received.InOrder(() =>
            {
                webClientMock.DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
                webClientFactoryMock.Release(webClientMock);
            });
        }

        [Test]
        public void DownloadFile_WhenInvoked_GetsFromUri()
        {
            systemUnderTest.DownloadFile(filePayload);

            webClientMock.Received(1).DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
        }

        [Test]
        public void DownloadFile_WhenInvoked_ReleasesWebClient()
        {
            systemUnderTest.DownloadFile(filePayload);

            Received.InOrder(() =>
            {
                webClientMock.DownloadFile(filePayload.DownloadUri, filePayload.DownloadFilePath);
                webClientFactoryMock.Release(webClientMock);
            });
        }

        [Test]
        public void DownloadFile_WhenInvoked_SetsTimeout()
        {
            systemUnderTest.DownloadFile(filePayload);

            webClientMock.Received(1).Timeout = TimeSpan.FromSeconds(filePayload.TimeoutSeconds);
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