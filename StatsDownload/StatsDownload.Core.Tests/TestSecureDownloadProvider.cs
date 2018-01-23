namespace StatsDownload.Core.Tests
{
    using System;
    using System.Net;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Logging;

    [TestFixture]
    public class TestSecureDownloadProvider
    {
        private IDownloadService downloadServiceMock;

        private FilePayload filePayload;

        private ILoggingService loggingServiceMock;

        private ISecureFilePayloadService secureFilePayloadServiceMock;

        private IDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => NewSecureDownloadProvider(null, secureFilePayloadServiceMock, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewSecureDownloadProvider(downloadServiceMock, null, loggingServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewSecureDownloadProvider(downloadServiceMock, secureFilePayloadServiceMock, null));
        }

        [Test]
        public void DownloadFile_WhenSecureConnectionType_OnlyTrySecureDownload()
        {
            var webException = new WebException();

            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(true);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(info =>
            {
                if (firstCall)
                {
                    firstCall = false;
                    throw webException;
                }
            });

            Assert.Throws(Is.EqualTo(webException), () => systemUnderTest.DownloadFile(filePayload));

            secureFilePayloadServiceMock.DidNotReceive().DisableSecureFilePayload(Arg.Any<FilePayload>());
            loggingServiceMock.Received().LogException(webException);
        }

        [Test]
        public void DownloadFile_WhenSecureConnectionTypeAndFirstDownloadThrowsException_ExceptionThrown()
        {
            var expected = new Exception();

            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(true);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(info =>
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

            Received.InOrder((() =>
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
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(info =>
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
            var webException = new WebException();

            secureFilePayloadServiceMock.IsSecureConnection(filePayload).Returns(false);

            var firstCall = true;
            downloadServiceMock.When(service => service.DownloadFile(filePayload)).Do(info =>
            {
                if (firstCall)
                {
                    firstCall = false;
                    throw webException;
                }
            });

            systemUnderTest.DownloadFile(filePayload);

            Received.InOrder((() =>
            {
                secureFilePayloadServiceMock.EnableSecureFilePayload(filePayload);
                downloadServiceMock.DownloadFile(filePayload);
                loggingServiceMock.LogException(webException);
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

            loggingServiceMock = Substitute.For<ILoggingService>();

            systemUnderTest = NewSecureDownloadProvider(downloadServiceMock, secureFilePayloadServiceMock,
                loggingServiceMock);
        }

        private IDownloadService NewSecureDownloadProvider(IDownloadService downloadService,
                                                           ISecureFilePayloadService secureFilePayloadService,
                                                           ILoggingService loggingService)
        {
            return new SecureDownloadProvider(downloadService, secureFilePayloadService, loggingService);
        }
    }
}