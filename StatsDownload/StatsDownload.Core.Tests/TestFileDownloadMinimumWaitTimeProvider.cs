namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadMinimumWaitTimeProvider
    {
        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private IFileDownloadMinimumWaitTimeService systemUnderTest;

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedLongEnough_ReturnsTrue()
        {
            bool actual = systemUnderTest.IsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [SetUp]
        public void SetUp()
        {
            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();
            fileDownloadDataStoreServiceMock.GetLastSuccessfulFileDownloadDateTime().Returns(DateTime.MinValue);

            systemUnderTest = new FileDownloadMinimumWaitTimeProvider(fileDownloadDataStoreServiceMock);
        }
    }
}