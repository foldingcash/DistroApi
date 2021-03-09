namespace StatsDownload.Core.Tests
{
    using System;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestSecureFilePayloadProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            loggerMock = Substitute.For<ILogger<SecureFilePayloadProvider>>();

            systemUnderTest = NewSecureHttpFilePayloadProvider(loggerMock);
        }

        private FilePayload filePayload;

        private ILogger<SecureFilePayloadProvider> loggerMock;

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
            loggerMock.Received()
                      .LogDebug(
                          $"Changing scheme https to http{Environment.NewLine}Old Uri: https://localhost/{Environment.NewLine}New Uri: http://localhost/");
        }

        [Test]
        public void EnableSecureFilePayload_WhenHttpConnection_ChangesToHttps()
        {
            filePayload.DownloadUri = new Uri("http://localhost/");

            systemUnderTest.EnableSecureFilePayload(filePayload);

            Assert.That(filePayload.DownloadUri.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
            loggerMock.Received()
                      .LogDebug(
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

        private ISecureFilePayloadService NewSecureHttpFilePayloadProvider(ILogger<SecureFilePayloadProvider> logger)
        {
            return new SecureFilePayloadProvider(logger);
        }
    }
}