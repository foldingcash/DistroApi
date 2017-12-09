namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadProvider
    {
        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFileDownloadLoggingService fileDownloadLoggingServiceMock;

        private IFileDownloadService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(null, fileDownloadLoggingServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadProvider(fileDownloadDataStoreServiceMock, null));
        }

        [Test]
        public void DownloadFile_WhenDataStoreIsNotAvailable_ReturnsFailedResultWithReason()
        {
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(false);

            var actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Exception, Is.Null);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_ExceptionHandled()
        {
            var expected = new Exception();
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw expected; });

            var actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.Exception, Is.EqualTo(expected));
        }

        [Test]
        public void DownloadFile_WhenExceptionThrown_LoggingIsCalled()
        {
            fileDownloadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw new Exception(); });

            InvokeDownloadFile();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [Test]
        public void DownloadFile_WhenSuccess_LoggingIsCalled()
        {
            InvokeDownloadFile();

            Received.InOrder(
                (() =>
                    {
                        fileDownloadLoggingServiceMock.LogVerbose("DownloadFile Invoked");
                        fileDownloadDataStoreServiceMock.IsAvailable();
                        fileDownloadLoggingServiceMock.LogResult(Arg.Any<FileDownloadResult>());
                    }));
        }

        [Test]
        public void DownloadFile_WhenSuccess_ResultIsSuccess()
        {
            var actual = InvokeDownloadFile();

            Assert.That(actual.Success, Is.True);
        }

        [SetUp]
        public void SetUp()
        {
            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.IsAvailable().Returns(true);

            fileDownloadLoggingServiceMock = Substitute.For<IFileDownloadLoggingService>();

            systemUnderTest = NewFileDownloadProvider(fileDownloadDataStoreServiceMock, fileDownloadLoggingServiceMock);
        }

        private FileDownloadResult InvokeDownloadFile()
        {
            return systemUnderTest.DownloadFile();
        }

        private IFileDownloadService NewFileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService)
        {
            return new FileDownloadProvider(fileDownloadDataStoreService, fileDownloadLoggingService);
        }
    }
}