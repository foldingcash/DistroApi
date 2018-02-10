namespace StatsDownload.Core.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Email;

    [TestFixture]
    public class TestFileDownloadEmailProvider
    {
        private IEmailService emailServiceMock;

        private IErrorMessageService errorMessageServiceMock;

        private IFileDownloadEmailService systemUnderTest;

        [Test]
        public void SendEmail_WhenInvoked_SendsEmail()
        {
            var filePayload = new FilePayload();
            errorMessageServiceMock.GetErrorMessage(FailedReason.UnexpectedException, filePayload)
                                   .Returns("ErrorMessage");

            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, filePayload));

            emailServiceMock.Received().SendEmail("File Download Failed", "ErrorMessage");
        }

        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            errorMessageServiceMock = Substitute.For<IErrorMessageService>();

            systemUnderTest = new FileDownloadEmailProvider(emailServiceMock, errorMessageServiceMock);
        }
    }
}