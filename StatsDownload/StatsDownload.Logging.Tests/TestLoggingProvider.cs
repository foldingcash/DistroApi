namespace StatsDownload.Logging.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestLoggingProvider
    {
        private IApplicationLoggingService applicationLoggingServiceMock;

        private ILoggingService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewLoggingProvider(null));
        }

        [Test]
        public void LogError_WhenInvoked_LogsError()
        {
            systemUnderTest.LogError("error");

            applicationLoggingServiceMock.Received().LogError("error");
        }

        [Test]
        public void LogException_WhenInvoked_LogsException()
        {
            var exception = new Exception("message");
            systemUnderTest.LogException(exception);

            applicationLoggingServiceMock.Received()
                                         .LogError($"Exception Type: {exception.GetType()}{Environment.NewLine}"
                                                   + $"Exception Message: {exception.Message}{Environment.NewLine}"
                                                   + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        [Test]
        public void LogVerbose_WhenInvoked_LogsVerbose()
        {
            systemUnderTest.LogVerbose("verbose");

            applicationLoggingServiceMock.Received().LogVerbose("verbose");
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
            applicationLoggingServiceMock = Substitute.For<IApplicationLoggingService>();

            systemUnderTest = NewLoggingProvider(applicationLoggingServiceMock);
        }

        private ILoggingService NewLoggingProvider(IApplicationLoggingService applicationLoggingService)
        {
            return new LoggingProvider(applicationLoggingService);
        }
    }
}