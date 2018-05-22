namespace StatsDownload.Logging.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;

    [TestFixture]
    public class TestLoggingProvider
    {
        private IApplicationLoggingService applicationLoggingServiceMock;

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private ILoggingService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewLoggingProvider(null, dateTimeServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewLoggingProvider(applicationLoggingServiceMock, null));
        }

        [Test]
        public void LogError_WhenInvoked_LogsError()
        {
            systemUnderTest.LogError("error");

            applicationLoggingServiceMock.Received().LogError($"{dateTime}{Environment.NewLine}error");
        }

        [Test]
        public void LogException_WhenInvoked_LogsException()
        {
            var exception = new Exception("message");
            systemUnderTest.LogException(exception);

            applicationLoggingServiceMock.Received()
                                         .LogError($"{dateTime}{Environment.NewLine}"
                                                   + $"Exception Type: {exception.GetType()}{Environment.NewLine}"
                                                   + $"Exception Message: {exception.Message}{Environment.NewLine}"
                                                   + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        [Test]
        public void LogVerbose_WhenInvoked_LogsVerbose()
        {
            systemUnderTest.LogVerbose("verbose");

            applicationLoggingServiceMock.Received().LogVerbose($"{dateTime}{Environment.NewLine}verbose");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void LogVerbose_WhenInvokedWithNoMessage_DoesNotLogVerbose(string noMessage)
        {
            systemUnderTest.LogVerbose(noMessage);

            applicationLoggingServiceMock.DidNotReceiveWithAnyArgs().LogVerbose(noMessage);
        }

        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Now;

            applicationLoggingServiceMock = Substitute.For<IApplicationLoggingService>();
            dateTimeServiceMock = Substitute.For<IDateTimeService>();

            systemUnderTest = NewLoggingProvider(applicationLoggingServiceMock, dateTimeServiceMock);

            dateTimeServiceMock.DateTimeNow().Returns(dateTime);
        }

        private ILoggingService NewLoggingProvider(IApplicationLoggingService applicationLoggingService,
                                                   IDateTimeService dateTimeService)
        {
            return new LoggingProvider(applicationLoggingService, dateTimeService);
        }
    }
}