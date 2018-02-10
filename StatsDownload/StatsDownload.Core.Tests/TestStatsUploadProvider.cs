namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestStatsUploadProvider
    {
        private IStatsUploadLoggingService loggingServiceMock;

        private IStatsUploadDataStoreService statsUploadDataStoreServiceMock;

        private IStatsUploadService systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            statsUploadDataStoreServiceMock = Substitute.For<IStatsUploadDataStoreService>();
            statsUploadDataStoreServiceMock.IsAvailable().Returns(true);

            loggingServiceMock = Substitute.For<IStatsUploadLoggingService>();

            systemUnderTest = new StatsUploadProvider(statsUploadDataStoreServiceMock, loggingServiceMock);
        }

        [Test]
        public void UploadStatsFile_WhenDataStoreIsNotAvailable_ReturnsDataStoreUnavailableResult()
        {
            statsUploadDataStoreServiceMock.IsAvailable().Returns(false);

            StatsUploadResult actual = InvokeUploadStatsFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.DataStoreUnavailable));
        }

        [Test]
        public void UploadStatsFile_WhenExceptionThrown_LogsException()
        {
            Exception expected = SetUpWhenExceptionThrown();

            InvokeUploadStatsFile();

            Received.InOrder(() =>
            {
                loggingServiceMock.LogVerbose("UploadStatsFile Invoked");
                statsUploadDataStoreServiceMock.IsAvailable();
                loggingServiceMock.LogException(expected);
            });
        }

        [Test]
        public void UploadStatsFile_WhenExceptionThrown_ReturnsUnexpectedExceptionResult()
        {
            SetUpWhenExceptionThrown();

            StatsUploadResult actual = InvokeUploadStatsFile();

            Assert.That(actual.Success, Is.False);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.UnexpectedException));
        }

        [Test]
        public void UploadStatsFile_WhenInvoked_ReturnsStatsUploadResult()
        {
            StatsUploadResult actual = InvokeUploadStatsFile();

            Assert.IsInstanceOf<StatsUploadResult>(actual);
        }

        [Test]
        public void UploadStatsFile_WhenInvoked_ReturnsSuccessResult()
        {
            StatsUploadResult actual = InvokeUploadStatsFile();

            Assert.That(actual.Success, Is.True);
            Assert.That(actual.FailedReason, Is.EqualTo(FailedReason.None));
        }

        private StatsUploadResult InvokeUploadStatsFile()
        {
            return systemUnderTest.UploadStatsFile();
        }

        private Exception SetUpWhenExceptionThrown()
        {
            var expected = new Exception();
            statsUploadDataStoreServiceMock.When(mock => mock.IsAvailable()).Do(info => { throw expected; });
            return expected;
        }
    }
}