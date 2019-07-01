namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;
    using NSubstitute.ClearExtensions;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Email;

    [TestFixture]
    public class TestStatsDownloadEmailProvider
    {
        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            emailSettingsServiceMock = Substitute.For<IEmailSettingsService>();
            emailSettingsServiceMock.GetFromDisplayName().Returns("DisplayName");

            systemUnderTest = NewStatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock,
                emailSettingsServiceMock);
        }

        private IEmailService emailServiceMock;

        private IEmailSettingsService emailSettingsServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IStatsDownloadEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(null, errorMessageServiceMock, emailSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(emailServiceMock, null, emailSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock, null));
        }

        [Test]
        public void SendEmail_WhenInvokedWithFailedUsersData_SendsEmail()
        {
            var failedUsersData = new FailedUserData[0];
            errorMessageServiceMock.GetErrorMessage(failedUsersData).Returns("ErrorMessage");

            systemUnderTest.SendEmail(failedUsersData);

            emailServiceMock.Received(1).SendEmail("DisplayName - User Data Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithFileDownloadResult_SendsEmail()
        {
            var filePayload = new FilePayload();
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, filePayload,
                StatsDownloadService.FileDownload).Returns("ErrorMessage");

            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, filePayload));

            emailServiceMock.Received().SendEmail("DisplayName - File Download Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResult_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, StatsDownloadService.StatsUpload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResult(1, FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("DisplayName - Stats Upload Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResults_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, StatsDownloadService.StatsUpload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResults(FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("DisplayName - Stats Upload Failed", "ErrorMessage");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        [TestCase("\n")]
        public void SendTestEmail_WhenEmptyDisplayName_DoesNotPrependDisplayName(string displayName)
        {
            emailSettingsServiceMock.ClearSubstitute();
            emailSettingsServiceMock.GetFromDisplayName().Returns(displayName);

            systemUnderTest.SendTestEmail();

            emailServiceMock.Received().SendEmail("Test Email", "This is a test email.");
        }

        [Test]
        public void SendTestEmail_WhenInvoked_SendsTestEmail()
        {
            systemUnderTest.SendTestEmail();

            emailServiceMock.Received().SendEmail("DisplayName - Test Email", "This is a test email.");
        }

        private IStatsDownloadEmailService NewStatsDownloadEmailProvider(IEmailService emailService,
                                                                         IErrorMessageService errorMessageService,
                                                                         IEmailSettingsService emailSettingsService)
        {
            return new StatsDownloadEmailProvider(emailService, errorMessageService, emailSettingsService);
        }
    }
}