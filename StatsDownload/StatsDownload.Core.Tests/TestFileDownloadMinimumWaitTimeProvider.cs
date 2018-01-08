namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadMinimumWaitTimeProvider
    {
        private IFileDownloadDataStoreService fileDownloadDataStoreServiceMock;

        private FilePayload filePayload;

        private IFileDownloadMinimumWaitTimeService systemUnderTest;

        [Test]
        public void IsMinimumWaitTimeMet_WhenConfiguredLessThanMinimum_UsesMinimum()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(0, 50, 0);
            fileDownloadDataStoreServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedConfiguredTime_ReturnsFalse()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDataStoreServiceMock.GetLastFileDownloadDateTime()
                .Returns(DateTime.Now.AddHours(-1).AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedMinimumTime_ReturnsFalse()
        {
            fileDownloadDataStoreServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedConfiguredTime_ReturnsTrue()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDataStoreServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddHours(-2));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedMinimumTime_ReturnsTrue()
        {
            fileDownloadDataStoreServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddHours(-1));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            fileDownloadDataStoreServiceMock = Substitute.For<IFileDownloadDataStoreService>();

            systemUnderTest = new FileDownloadMinimumWaitTimeProvider(fileDownloadDataStoreServiceMock);
        }

        private bool InvokeIsMinimumWaitTimeMet()
        {
            return systemUnderTest.IsMinimumWaitTimeMet(filePayload);
        }
    }
}