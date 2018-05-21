namespace StatsDownload.Core.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Logging;

    [TestFixture]
    public class TestDownloadProvider
    {
        private IDateTimeService dateTimeServiceMock;

        private FilePayload filePayload;

        private IFileService fileServiceMock;

        private IHttpClientFactory httpClientFactoryMock;

        private IHttpClient httpClientMock;

        private IHttpContent httpContentMock;

        private IHttpResponseMessage httpResponseMessageMock;

        private ILoggingService loggingServiceMock;

        private Stream stream;

        private DownloadProvider systemUnderTest;

        [Test]
        public void DownloadFile_WhenInvoked_GetsFromUri()
        {
            systemUnderTest.DownloadFile(filePayload);

            httpClientMock.Received(1).GetAsync(filePayload.DownloadUri);
        }

        [Test]
        public void DownloadFile_WhenInvoked_SetsTimeout()
        {
            systemUnderTest.DownloadFile(filePayload);

            httpClientMock.Received(1).Timeout = TimeSpan.FromSeconds(filePayload.TimeoutSeconds);
        }

        [Test]
        public void DownloadFile_WhenNotSuccessful_Throws()
        {
            httpResponseMessageMock.StatusCode.Returns(HttpStatusCode.NotFound);
            httpResponseMessageMock.IsSuccessStatusCode.Returns(false);

            Assert.That(() => { systemUnderTest.DownloadFile(filePayload); },
                Throws.InstanceOf<Exception>().With.Message
                      .EqualTo("Failed to download the target file. Status Code: NotFound"));
        }

        [Test]
        public void DownloadFile_WhenSuccessful_SavesToFile()
        {
            systemUnderTest.DownloadFile(filePayload);

            fileServiceMock.Received(1).CreateFromStream(filePayload.DownloadFilePath, stream);
        }

        [SetUp]
        public void SetUp()
        {
            loggingServiceMock = Substitute.For<ILoggingService>();
            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            fileServiceMock = Substitute.For<IFileService>();
            filePayload = GenerateFilePayload();

            stream = new MemoryStream();
            httpContentMock = Substitute.For<IHttpContent>();
            httpContentMock.ReadAsStreamAsync().Returns(Task.FromResult(stream));
            httpResponseMessageMock = Substitute.For<IHttpResponseMessage>();
            httpResponseMessageMock.Content.Returns(httpContentMock);
            httpResponseMessageMock.IsSuccessStatusCode.Returns(true);
            httpClientMock = Substitute.For<IHttpClient>();
            httpClientMock.GetAsync(Arg.Any<Uri>()).Returns(Task.FromResult(httpResponseMessageMock));
            httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            httpClientFactoryMock.Create().Returns(httpClientMock);

            systemUnderTest = new DownloadProvider(loggingServiceMock, dateTimeServiceMock, httpClientFactoryMock,
                fileServiceMock);
        }

        [TearDown]
        public void TearDown()
        {
            stream.Dispose();
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