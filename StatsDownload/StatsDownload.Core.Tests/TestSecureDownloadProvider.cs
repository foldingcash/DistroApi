namespace StatsDownload.Core.Tests
{
    using System;
    using System.Net;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestSecureDownloadProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            downloadServiceMock = Substitute.For<IDownloadService>();

            secureFilePayloadServiceMock = Substitute.For<ISecureFilePayloadService>();

            loggerMock = Substitute.For<ILogger<SecureDownloadProvider>>();

            systemUnderTest = NewSecureDownloadProvider(loggerMock, downloadServiceMock, secureFilePayloadServiceMock);
        }

        private IDownloadService downloadServiceMock;

        private FilePayload filePayload;

        private ILogger<SecureDownloadProvider> loggerMock;

        private ISecureFilePayloadService secureFilePayloadServiceMock;

        private IDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewSecureDownloadProvider(null, downloadServiceMock, secureFilePayloadServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewSecureDownloadProvider(loggerMock, null, secureFilePayloadServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewSecureDownloadProvider(loggerMock, downloadServiceMock, null));
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
            loggerMock.Received(1).LogError(webException,
                "There was a web exception while trying to securely download the file");
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

            Received.InOrder(() =>
            {
                secureFilePayloadServiceMock.EnableSecureFilePayload(filePayload);
                downloadServiceMock.DownloadFile(filePayload);
            });
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

            Received.InOrder(() =>
            {
                secureFilePayloadServiceMock.EnableSecureFilePayload(filePayload);
                downloadServiceMock.DownloadFile(filePayload);
                loggerMock.LogError(webException,
                    "There was a web exception while trying to securely download the file");
                secureFilePayloadServiceMock.DisableSecureFilePayload(filePayload);
                downloadServiceMock.DownloadFile(filePayload);
            });
        }

        private IDownloadService NewSecureDownloadProvider(ILogger<SecureDownloadProvider> logger,
                                                           IDownloadService downloadService,
                                                           ISecureFilePayloadService secureFilePayloadService)
        {
            return new SecureDownloadProvider(logger, downloadService, secureFilePayloadService);
        }
    }
}