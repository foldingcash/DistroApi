namespace StatsDownload.Core.Tests
{
    using System.Collections.Generic;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Email;

    [TestFixture]
    public class TestStatsDownloadEmailProvider
    {
        private IEmailService emailServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IStatsDownloadEmailService systemUnderTest;

        [Test]
        public void SendEmail_WhenInvokedWithFailedUsersData_SendsEmail()
        {
            var failedUsersData = new List<FailedUserData>();
            errorMessageServiceMock.GetErrorMessage(failedUsersData).Returns("ErrorMessage");

            systemUnderTest.SendEmail(failedUsersData);

            emailServiceMock.Received(1).SendEmail("User Data Failed Parsing", "ErrorMessage");
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

        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = new StatsDownloadEmailProvider(emailServiceMock, errorMessageServiceMock);
        }
    }
}