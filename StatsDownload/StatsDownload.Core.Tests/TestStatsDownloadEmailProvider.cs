namespace StatsDownload.Core.Tests
{
    using System;
    using Email;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;
    using NSubstitute;
    using NUnit.Framework;

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
                                       StatsDownloadService.FileDownload)
                                   .Returns("ErrorMessage");

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

        [Test]
        public void SendTestEmail_WhenInvoked_SendsTestEmail()
        {
            systemUnderTest.SendTestEmail();

            emailServiceMock
                .Received().SendEmail("DisplayName - Test Email", "This is a test email.");
        }

        private IStatsDownloadEmailService NewStatsDownloadEmailProvider(IEmailService emailService,
            IErrorMessageService errorMessageService,
            IEmailSettingsService emailSettingsService)
        {
            return new StatsDownloadEmailProvider(emailService, errorMessageService, emailSettingsService);
        }
    }
}