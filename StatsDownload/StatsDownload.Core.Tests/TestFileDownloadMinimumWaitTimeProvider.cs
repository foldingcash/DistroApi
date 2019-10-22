namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestFileDownloadMinimumWaitTimeProvider
    {
        [SetUp]
        public void SetUp()
        {
            filePayload = new FilePayload();
            dateTime = DateTime.UtcNow;

            fileDownloadDatabaseServiceMock = Substitute.For<IFileDownloadDatabaseService>();

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            systemUnderTest =
                NewFileDownloadMinimumWaitTimeProvider(fileDownloadDatabaseServiceMock, dateTimeServiceMock);
        }

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private IFileDownloadDatabaseService fileDownloadDatabaseServiceMock;

        private FilePayload filePayload;

        private IFileDownloadMinimumWaitTimeService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => NewFileDownloadMinimumWaitTimeProvider(null, dateTimeServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewFileDownloadMinimumWaitTimeProvider(fileDownloadDatabaseServiceMock, null));
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenConfiguredLessThanMinimum_UsesMinimum()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(0, 50, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(dateTime.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedConfiguredTime_ReturnsFalse()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime()
                                           .Returns(dateTime.AddHours(-1).AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenNotWaitedMinimumTime_ReturnsFalse()
        {
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(dateTime.AddMinutes(-59));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedConfiguredTime_ReturnsTrue()
        {
            filePayload.MinimumWaitTimeSpan = new TimeSpan(2, 0, 0);
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(dateTime.AddHours(-2));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsMinimumWaitTimeMet_WhenWaitedMinimumTime_ReturnsTrue()
        {
            fileDownloadDatabaseServiceMock.GetLastFileDownloadDateTime().Returns(dateTime.AddHours(-1));

            bool actual = InvokeIsMinimumWaitTimeMet();

            Assert.That(actual, Is.True);
        }

        private bool InvokeIsMinimumWaitTimeMet()
        {
            return systemUnderTest.IsMinimumWaitTimeMet(filePayload);
        }

        private IFileDownloadMinimumWaitTimeService NewFileDownloadMinimumWaitTimeProvider(
            IFileDownloadDatabaseService fileDownloadDatabaseService, IDateTimeService dateTimeService)
        {
            return new FileDownloadMinimumWaitTimeProvider(fileDownloadDatabaseService, dateTimeService);
        }
    }
}