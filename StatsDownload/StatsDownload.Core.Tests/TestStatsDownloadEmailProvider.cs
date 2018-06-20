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

            systemUnderTest = NewStatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock);
        }

        private IEmailService emailServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IStatsDownloadEmailService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadEmailProvider(null, errorMessageServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewStatsDownloadEmailProvider(emailServiceMock, null));
        }

        [Test]
        public void SendEmail_WhenInvokedWithFailedUsersData_SendsEmail()
        {
            var failedUsersData = new List<FailedUserData>();
            errorMessageServiceMock.GetErrorMessage(failedUsersData).Returns("ErrorMessage");

            systemUnderTest.SendEmail(failedUsersData);

            emailServiceMock.Received(1).SendEmail("User Data Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithFileDownloadResult_SendsEmail()
        {
            var filePayload = new FilePayload();
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, filePayload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, filePayload));

            emailServiceMock.Received().SendEmail("File Download Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResult_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException).Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResult(1, FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("Stats Upload Failed", "ErrorMessage");
        }

        [Test]
        public void SendEmail_WhenInvokedWithStatsUploadResults_SendsEmail()
        {
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException).Returns("ErrorMessage");

            systemUnderTest.SendEmail(new StatsUploadResults(FailedReason.UnexpectedException));

            emailServiceMock.Received().SendEmail("Stats Upload Failed", "ErrorMessage");
        }

        private IStatsDownloadEmailService NewStatsDownloadEmailProvider(IEmailService emailService,
            IErrorMessageService errorMessageService)
        {
            return new StatsDownloadEmailProvider(emailService, errorMessageService);
        }
    }
}