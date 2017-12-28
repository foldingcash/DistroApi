namespace StatsDownload.Core.Tests
{
    using System;
    using System.Net;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestSecureDownloadProvider
    {
        private IDownloadService downloadServiceMock;

        private FilePayload filePayload;

        private ISecureFilePayloadService secureFilePayloadServiceMock;

        private IDownloadService systemUnderTest;

        [Test]
        public void DownloadFile_WhenSecureConnectionType_OnlyTrySecureDownload()
        {
            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(true);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(
                info =>
                    {
                        if (firstCall)
                        {
                            firstCall = false;
                            throw new WebException();
                        }
                    });

            systemUnderTest.DownloadFile(filePayload);

            secureFilePayloadServiceMock.DidNotReceive().DisableSecureFilePayload(Arg.Any<FilePayload>());
        }

        [Test]
        public void DownloadFile_WhenSecureConnectionTypeAndFirstDownloadThrowsException_ExceptionThrown()
        {
            var expected = new Exception();

            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(true);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(
                info =>
                    {
                        if (firstCall)
                        {
                            firstCall = false;
                            throw expected;
                        }
                    });

            Assert.Throws(Is.EqualTo(expected), () => systemUnderTest.DownloadFile(filePayload));
        }

        [Test]
        public void DownloadFile_WhenUnsecureConnectionType_TrySecureDownloadFirst()
        {
            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(false);

            systemUnderTest.DownloadFile(filePayload);

            Received.InOrder(
                (() =>
                    {
                        secureFilePayloadServiceMock.EnableSecureFilePayload(filePayload);
                        downloadServiceMock.DownloadFile(filePayload);
                    }));
        }

        [Test]
        public void DownloadFile_WhenUnsecureConnectionTypeAndFirstDownloadThrowsException_ExceptionThrown()
        {
            var expected = new Exception();

            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(false);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(
                info =>
                    {
                        if (firstCall)
                        {
                            firstCall = false;
                            throw expected;
                        }
                    });

            Assert.Throws(Is.EqualTo(expected), () => systemUnderTest.DownloadFile(filePayload));
        }

        [Test]
        public void DownloadFile_WhenUnsecureConnectionTypeAndFirstDownloadThrowsWebException_TryUnsecureDownload()
        {
            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(false);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(
                info =>
                    {
                        if (firstCall)
                        {
                            firstCall = false;
                            throw new WebException();
                        }
                    });

            systemUnderTest.DownloadFile(filePayload);

            Received.InOrder(
                (() =>
                    {
                        secureFilePayloadServiceMock.EnableSecureFilePayload(filePayload);
                        downloadServiceMock.DownloadFile(filePayload);
                        secureFilePayloadServiceMock.DisableSecureFilePayload(filePayload);
                        downloadServiceMock.DownloadFile(filePayload);
                    }));
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            downloadServiceMock = Substitute.For<IDownloadService>();

            secureFilePayloadServiceMock = Substitute.For<ISecureFilePayloadService>();

            systemUnderTest = new SecureDownloadProvider(downloadServiceMock, secureFilePayloadServiceMock);
        }
    }
}