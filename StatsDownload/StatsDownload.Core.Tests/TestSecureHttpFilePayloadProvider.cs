namespace StatsDownload.Core.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class TestSecureHttpFilePayloadProvider
    {
        private FilePayload filePayload;

        private ISecureFilePayloadService systemUnderTest;

        [Test]
        public void DisableSecureFilePayload_WhenHttpsConnection_ChangesToHttp()
        {
            filePayload.DownloadUri = new Uri("https://localhost/");

            systemUnderTest.DisableSecureFilePayload(filePayload);

            Assert.That(filePayload.DownloadUri.Scheme, Is.EqualTo(Uri.UriSchemeHttp));
        }

        [Test]
        public void EnableSecureFilePayload_WhenHttpConnection_ChangesToHttps()
        {
            filePayload.DownloadUri = new Uri("http://localhost/");

            systemUnderTest.EnableSecureFilePayload(filePayload);

            Assert.That(filePayload.DownloadUri.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
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

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            systemUnderTest = new SecureHttpFilePayloadProvider();
        }
    }
}