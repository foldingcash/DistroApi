namespace StatsDownload.Core.Tests
{
    using System;

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

            emailServiceMock.Received()
                .SendEmail(
                    "File Download Failed",
                    "There was a problem downloading the file payload. The data store is unavailable, ensure the data store is available and configured correctly and try again.");
        }

        [Test]
        public void SendEmail_WhenInvokedWithMinimumWaitTimeNotMet_SendsEmail()
        {
            var configuredWaitTime = new TimeSpan(1, 0, 0, 0);

            systemUnderTest.SendEmail(
                new FileDownloadResult(
                    FailedReason.MinimumWaitTimeNotMet,
                    new FilePayload { MinimumWaitTimeSpan = configuredWaitTime }));

            emailServiceMock.Received()
                .SendEmail(
                    "File Download Failed",
                    $"There was a problem downloading the file payload. The file download service was run before the minimum wait time {MinimumWait.TimeSpan} or the configured wait time {configuredWaitTime}. "
                    + "Configure to run the service less often or decrease your configured wait time and try again.");
        }

        [Test]
        public void SendEmail_WhenInvokedWithUnexpectedException_SendsEmail()
        {
            systemUnderTest.SendEmail(new FileDownloadResult(FailedReason.UnexpectedException, new FilePayload()));

            emailServiceMock.Received()
                .SendEmail(
                    "File Download Failed",
                    "There was a catastrophic problem downloading the file payload. Check the log for more information.");
        }

        [SetUp]
        public void SetUp()
        {
            emailServiceMock = Substitute.For<IEmailService>();

            systemUnderTest = new FileDownloadEmailProvider(emailServiceMock);
        }
    }
}