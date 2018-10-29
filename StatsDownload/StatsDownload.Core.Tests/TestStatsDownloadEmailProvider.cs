namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
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

            statsDownloadEmailSettingsServiceMock = Substitute.For<IStatsDownloadEmailSettingsService>();
            statsDownloadEmailSettingsServiceMock.GetInstanceName().Returns("InstanceName");

            systemUnderTest = NewStatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock,
                statsDownloadEmailSettingsServiceMock);
        }

        private IEmailService emailServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IStatsDownloadEmailSettingsService statsDownloadEmailSettingsServiceMock;

        private IStatsDownloadEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(null, errorMessageServiceMock, statsDownloadEmailSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(emailServiceMock, null, statsDownloadEmailSettingsServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock, null));
        }

        [Test]
        public void SendEmail_WhenInvokedWithFailedUsersData_SendsEmail()
        {
            var failedUsersData = new List<FailedUserData>();
            errorMessageServiceMock.GetErrorMessage(failedUsersData).Returns("ErrorMessage");

            systemUnderTest.SendEmail(failedUsersData);

            emailServiceMock.Received(1).SendEmail("InstanceName - User Data Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithFileDownloadResult_SendsEmail()
        {
            var filePayload = new FilePayload();
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, filePayload,
                                       StatsDownloadService.FileDownload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, filePayload));

            emailServiceMock.Received().SendEmail("InstanceName - File Download Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResult_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, StatsDownloadService.StatsUpload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResult(1, FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("InstanceName - Stats Upload Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResults_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, StatsDownloadService.StatsUpload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResults(FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("InstanceName - Stats Upload Failed", "ErrorMessage");
        }

        [Test]
        public void SendTestEmail_WhenInvoked_SendsTestEmail()
        {
            systemUnderTest.SendTestEmail();

            emailServiceMock
                .Received().SendEmail("InstanceName - Test Email", "This is a test email.");
        }

        private IStatsDownloadEmailService NewStatsDownloadEmailProvider(IEmailService emailService,
            IErrorMessageService errorMessageService,
            IStatsDownloadEmailSettingsService statsDownloadEmailSettingsService)
        {
            return new StatsDownloadEmailProvider(emailService, errorMessageService, statsDownloadEmailSettingsService);
        }
    }
}