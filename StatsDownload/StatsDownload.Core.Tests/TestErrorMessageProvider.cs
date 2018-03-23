namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    [TestFixture]
    public class TestErrorMessageProvider
    {
        private IErrorMessageService systemUnderTest;

        [Test]
        public void GetErrorMessage_WhenDataStoreUnavailable_ReturnsDataStoreUnavailableMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.DataStoreUnavailable, new FilePayload());

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. The data store is unavailable, ensure the data store is available and configured correctly and try again."));
        }

        [Test]
        public void GetErrorMessage_WhenFailedUserData_ReturnsFailedUserDataMessage()
        {
            var failedUsersData = new List<FailedUserData> { new FailedUserData(), new FailedUserData() };
            string actual = systemUnderTest.GetErrorMessage(failedUsersData);

            Assert.That(actual,
                Is.EqualTo(
                    $"There was a problem uploading the file payload. The file passed validation but {failedUsersData.Count} lines failed validation; processing continued after encountering these lines. If this problem occurs again, then you should contact your technical advisor to review the logs and failed user parsings."));
        }

        [Test]
        public void GetErrorMessage_WhenFileDownloadFailedDecompression_ReturnsFileDownloadFailedDecompressionMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.FileDownloadFailedDecompression,
                new FilePayload());

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem decompressing the file payload. The file has been moved to a failed directory for review. If this problem occurs again, then you should contact your technical advisor to review the logs and failed files."));
        }

        [Test]
        public void GetErrorMessage_WhenFileDownloadTimeout_ReturnsFileDownloadTimeoutMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.FileDownloadTimeout, new FilePayload());

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. There was a timeout when downloading the file payload. If a timeout occurs again, then you can try increasing the configurable download timeout."));
        }

        [Test]
        public void GetErrorMessage_WhenInvalidStatsFile_ReturnsInvalidStatsFileMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.InvalidStatsFileUpload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem uploading the file payload. The file failed validation; check the logs for more information. If this problem occurs again, then you should contact your technical advisor to review the logs and failed uploads."));
        }

        [Test]
        public void GetErrorMessage_WhenMinimumWaitTimeNotMet_ReturnsMinimumWaitTimeNotMetMessage()
        {
            var configuredWaitTime = new TimeSpan(1, 0, 0, 0);

            string actual = systemUnderTest.GetErrorMessage(FailedReason.MinimumWaitTimeNotMet,
                new FilePayload { MinimumWaitTimeSpan = configuredWaitTime });

            Assert.That(actual,
                Is.EqualTo(
                    $"There was a problem downloading the file payload. The file download service was run before the minimum wait time {MinimumWait.TimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again."));
        }

        [Test]
        public void GetErrorMessage_WhenNoFailedReason_ReturnsEmptyMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.None, new FilePayload());

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetErrorMessage_WhenRequiredSettingsInvalid_ReturnsRequiredSettingsInvalidMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.RequiredSettingsInvalid, new FilePayload());

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. The required settings are invalid; check the logs for more information. Ensure the settings are complete and accurate, then try again."));
        }

        [Test]
        public void GetErrorMessage_WhenUnexpectedException_ReturnsUnexpectedExceptionMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.UnexpectedException, new FilePayload());

            Assert.That(actual,
                Is.EqualTo("There was a problem downloading the file payload. Check the log for more information."));
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new ErrorMessageProvider();
        }
    }
}