namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestFileDownloadMinimumWaitTimeProvider
    {
        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private FilePayload filePayload;

        private IFileDownloadMinimumWaitTimeService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewFileDownloadMinimumWaitTimeProvider(null));
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenConfiguredLessThanMinimum_UsesMinimum()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(0, 50, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedConfiguredTime_ReturnsFalse()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime()
                                           .Returns(DateTime.Now.AddHours(-1).AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedMinimumTime_ReturnsFalse()
        {
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedConfiguredTime_ReturnsTrue()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddHours(-2));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedMinimumTime_ReturnsTrue()
        {
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(DateTime.Now.AddHours(-1));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();

            systemUnderTest = NewFileDownloadMinimumWaitTimeProvider(fileDownloadDatabaseServiceMock);
        }

        private bool InvokeIsMinimumWaitTimeMet()
        {
            return systemUnderTest.IsMinimumWaitTimeMet(filePayload);
        }

        private IFileDownloadMinimumWaitTimeService NewFileDownloadMinimumWaitTimeProvider(
            IFileDownloadDatabaseService fileDownloadDatabaseService)
        {
            return new FileDownloadMinimumWaitTimeProvider(fileDownloadDatabaseService);
        }
    }
}