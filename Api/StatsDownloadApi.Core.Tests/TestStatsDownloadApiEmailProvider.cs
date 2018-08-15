namespace StatsDownloadApi.Core.Tests
{
    using System;
    using Interfaces;
    using NSubstitute;
    using NUnit.Framework;
    using StatsDownload.Email;

    [TestFixture]
    public class TestStatsDownloadApiEmailProvider
    {
        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            systemUnderTest = NewStatsDownloadApiEmailProvider(emailServiceMock);
        }

        private IEmailService emailServiceMock;

        private IStatsDownloadApiEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiEmailProvider(null));
        }

        [Test]
        public void SendUnhandledExceptionEmail_WhenInvoked_SendsEmail()
        {
            var exception = new Exception("test message");

            systemUnderTest.SendUnhandledExceptionEmail(exception);

            emailServiceMock.Received().SendEmail("API Unhandled Exception Caught",
                "The StatsDownload API experienced an unhandled exception. Contact your technical advisor with the exception message. Exception Message: test message.");
        }

        private IStatsDownloadApiEmailService NewStatsDownloadApiEmailProvider(IEmailService emailService)
        {
            return new StatsDownloadApiEmailProvider(emailService);
        }
    }
}