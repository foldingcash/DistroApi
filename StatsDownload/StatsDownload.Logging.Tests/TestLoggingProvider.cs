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
                                         .LogError($"Exception Type: {exception.GetType()}"
                                                   + $"Exception Message: {exception.Message}"
                                                   + $"Exception Stack-trace: {Environment.NewLine}{exception.StackTrace}");
        }

        [Test]
        public void LogVerbose_WhenInvoked_LogsVerbose()
        {
            systemUnderTest.LogVerbose("verbose");

            applicationLoggingServiceMock.Received().LogVerbose("verbose");
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