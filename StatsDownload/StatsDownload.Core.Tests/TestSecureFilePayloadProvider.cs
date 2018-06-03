namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Logging;

    [TestFixture]
    public class TestSecureFilePayloadProvider
    {
        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private ISecureFilePayloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewSecureHttpFilePayloadProvider(null));
        }

        [Test]
        public void DisableSecureFilePayload_WhenHttpsConnection_ChangesToHttp()
        {
            filePayload.DownloadUri = new Uri("https://localhost/");

            systemUnderTest.DisableSecureFilePayload(filePayload);

            Assert.That(filePayload.DownloadUri.Scheme, Is.EqualTo(Uri.UriSchemeHttp));
            loggingServiceMock.Received()
                              .LogVerbose(
                                  $"Changing scheme https to http{Environment.NewLine}Old Uri: https://localhost/{Environment.NewLine}New Uri: http://localhost/");
        }

        [Test]
        public void EnableSecureFilePayload_WhenHttpConnection_ChangesToHttps()
        {
            filePayload.DownloadUri = new Uri("http://localhost/");

            systemUnderTest.EnableSecureFilePayload(filePayload);

            Assert.That(filePayload.DownloadUri.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
            loggingServiceMock.Received()
                              .LogVerbose(
                                  $"Changing scheme http to https{Environment.NewLine}Old Uri: http://localhost/{Environment.NewLine}New Uri: https://localhost/");
        }

        [Test]
        public void IsSecureConnection_WhenHttpsConnection_ReturnsTrue()
        {
            filePayload.DownloadUri = new Uri("https://localhost/");

            bool actual = systemUnderTest.IsSecureConnection(filePayload);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsSecureConnection_WhenNotHttpsConnection_ReturnsFalse()
        {
            filePayload.DownloadUri = new Uri("http://localhost/");

            bool actual = systemUnderTest.IsSecureConnection(filePayload);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsSecureConnection_WhenNotHttpTypeConnection_ReturnsTrue()
        {
            filePayload.DownloadUri = new Uri(@"C:\test.txt");

            bool actual = systemUnderTest.IsSecureConnection(filePayload);

            Assert.That(actual, Is.True);
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewSecureHttpFilePayloadProvider(loggingServiceMock);
        }

        private ISecureFilePayloadService NewSecureHttpFilePayloadProvider(ILoggingService loggingService)
        {
            return new SecureFilePayloadProvider(loggingService);
        }
    }
}