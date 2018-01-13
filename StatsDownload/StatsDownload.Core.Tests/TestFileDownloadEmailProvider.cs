namespace StatsDownload.Core.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Email;

    [TestFixture]
    public class TestFileDownloadEmailProvider
    {
        private IEmailService emailServiceMock;

        private IFileDownloadEmailService systemUnderTest;

        [Test]
        public void SendEmail_WhenInvokedWithDataStoreUnavailable_SendsEmail()
        {
            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.DataStoreUnavailable, new FilePayload()));

            emailServiceMock.Received().SendEmail("File Download Failed", "");
        }

        [Test]
        public void SendEmail_WhenInvokedWithMinimumWaitTimeNotMet_SendsEmail()
        {
            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.MinimumWaitTimeNotMet, new FilePayload()));

            emailServiceMock.Received().SendEmail("File Download Failed", "");
        }

        [Test]
        public void SendEmail_WhenInvokedWithUnexpectedException_SendsEmail()
        {
            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, new FilePayload()));

            emailServiceMock.Received().SendEmail("File Download Failed", "");
        }

        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            systemUnderTest = new FileDownloadEmailProvider(emailServiceMock);
        }
    }
}