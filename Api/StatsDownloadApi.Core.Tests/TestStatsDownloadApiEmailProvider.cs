namespace StatsDownloadApi.Core.Tests
{
    using System;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Email;

    using StatsDownloadApi.Interfaces;

    [TestFixture]
    public class TestStatsDownloadApiEmailProvider
    {
        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            emailSettings = new EmailSettings { DisplayName = "DisplayName" };

            emailSettingsOptionsMock = Substitute.For<IOptions<EmailSettings>>();
            emailSettingsOptionsMock.Value.Returns(emailSettings);

            systemUnderTest = NewStatsDownloadApiEmailProvider(emailServiceMock, emailSettingsOptionsMock);
        }

        private IEmailService emailServiceMock;

        private EmailSettings emailSettings;

        private IOptions<EmailSettings> emailSettingsOptionsMock;

        private IStatsDownloadApiEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadApiEmailProvider(null, emailSettingsOptionsMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadApiEmailProvider(emailServiceMock, null));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void SendUnhandledExceptionEmail_WhenEmptyDisplayName_DoesNotPrependDisplayName(string displayName)
        {
            emailSettings.DisplayName = displayName;

            var exception = new Exception("test message");

            systemUnderTest.SendUnhandledExceptionEmail(exception);

            emailServiceMock.Received().SendEmail("API Unhandled Exception Caught",
                "The StatsDownload API experienced an unhandled exception. Contact your technical advisor with the exception message. Exception Message: test message");
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
                                                                               IOptions<EmailSettings>
                                                                                   emailSettingsOptionsMock)
        {
            return new StatsDownloadApiEmailProvider(emailService, emailSettingsOptionsMock);
        }
    }
}