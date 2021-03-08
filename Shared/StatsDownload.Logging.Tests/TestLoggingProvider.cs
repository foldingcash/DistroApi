namespace StatsDownload.Logging.Tests
{
    using System;

    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;

    [TestFixture]
    public class TestLoggingProvider
    {
        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.UtcNow;
            
            dateTimeServiceMock = Substitute.For<IDateTimeService>();

            loggerMock = Substitute.For<ILogger<LoggingProvider>>();

            systemUnderTest = NewLoggingProvider(loggerMock, dateTimeServiceMock);

            dateTimeServiceMock.DateTimeNow().Returns(dateTime);
        }

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private ILoggingService systemUnderTest;

        private ILogger<LoggingProvider> loggerMock;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewLoggingProvider(null, dateTimeServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewLoggingProvider(loggerMock, null));
        }

        [Test]
        public void LogDebug_WhenInvoked_LogsDebug()
        {
            systemUnderTest.LogDebug("debug");

            loggerMock.Received().LogDebug($"{dateTime}{Environment.NewLine}debug");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void LogDebug_WhenInvokedWithNoMessage_DoesNotLogDebug(string noMessage)
        {
            systemUnderTest.LogDebug(noMessage);

            loggerMock.DidNotReceiveWithAnyArgs().LogDebug(noMessage);
        }

        [Test]
        public void LogError_WhenInvoked_LogsError()
        {
            systemUnderTest.LogError("error");

            loggerMock.Received().LogError($"{dateTime}{Environment.NewLine}error");
        }

        [Test]
        public void LogException_WhenInvoked_LogsException()
        {
            var exception = new Exception("message");
            systemUnderTest.LogException(exception);

            loggerMock.Received().LogError($"{dateTime}{Environment.NewLine}"
                                           + $"Exception Type: {exception.GetType()}{Environment.NewLine}"
                                           + $"Exception Message: {exception.Message}{Environment.NewLine}"
                                           + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        [Test]
        public void LogMethodFinishedInvoked_WhenInvoked_LogsMethodFinished()
        {
            systemUnderTest.LogMethodFinished();

            loggerMock.Received().LogDebug(
                $"{dateTime}{Environment.NewLine}{nameof(LogMethodFinishedInvoked_WhenInvoked_LogsMethodFinished)} Finished");
        }

        [Test]
        public void LogMethodInvoked_WhenInvoked_LogsMethodInvoked()
        {
            systemUnderTest.LogMethodInvoked();

            loggerMock.Received().LogDebug(
                $"{dateTime}{Environment.NewLine}{nameof(LogMethodInvoked_WhenInvoked_LogsMethodInvoked)} Invoked");
        }

        private ILoggingService NewLoggingProvider(ILogger<LoggingProvider> logger,
                                                   IDateTimeService dateTimeService)
        {
            return new LoggingProvider(logger, dateTimeService);
        }
    }
}