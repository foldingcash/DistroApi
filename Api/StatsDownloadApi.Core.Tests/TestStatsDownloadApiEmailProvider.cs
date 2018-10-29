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

            emailSettingsServiceMock = Substitute.For<IEmailSettingsService>();
            emailSettingsServiceMock.GetFromDisplayName().Returns("DisplayName");

            systemUnderTest = NewStatsDownloadApiEmailProvider(emailServiceMock, emailSettingsServiceMock);
        }

        private IEmailService emailServiceMock;

        private IEmailSettingsService emailSettingsServiceMock;

        private IStatsDownloadApiEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiEmailProvider(null, emailSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiEmailProvider(emailServiceMock, null));
        }

        [Test]
        public void SendUnhandledExceptionEmail_WhenInvoked_SendsEmail()
        {
            var exception = new Exception("test message");

            systemUnderTest.SendUnhandledExceptionEmail(exception);

            emailServiceMock.Received().SendEmail("DisplayName - API Unhandled Exception Caught",
                "The StatsDownload API experienced an unhandled exception. Contact your technical advisor with the exception message. Exception Message: test message");
        }

        private IStatsDownloadApiEmailService NewStatsDownloadApiEmailProvider(IEmailService emailService,
            IEmailSettingsService emailSettingsService)
        {
            return new StatsDownloadApiEmailProvider(emailService, emailSettingsService);
        }
    }
}