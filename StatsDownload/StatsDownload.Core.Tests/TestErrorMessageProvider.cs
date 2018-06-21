namespace StatsDownload.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using DataTransfer;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;
    using NUnit.Framework;

    [TestFixture]
    public class TestErrorMessageProvider
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new ErrorMessageProvider();
        }

        private IErrorMessageService systemUnderTest;

        [Test]
        public void
            GetErrorMessage_WhenDataStoreUnavailableDuringFileDownload_ReturnsFileDownloadDataStoreUnavailableMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.DataStoreUnavailable, new FilePayload(),
                StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. There was a problem connecting to the data store. The data store is unavailable, ensure the data store is available and configured correctly and try again."));
        }

        [Test]
        public void
            GetErrorMessage_WhenDataStoreUnavailableDuringStatsUpload_ReturnsStatsUploadDataStoreUnavailableMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.DataStoreUnavailable, new FilePayload(),
                StatsDownloadService.StatsUpload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem uploading the file payload. There was a problem connecting to the data store. The data store is unavailable, ensure the data store is available and configured correctly and try again."));
        }

        [Test]
        public void GetErrorMessage_WhenFailedAddToDatabase_ReturnsFailedAddToDatabaseMessage()
        {
            var failedUserData = new FailedUserData(0, RejectionReason.FailedAddToDatabase, new UserData());

            string actual = systemUnderTest.GetErrorMessage(failedUserData);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem adding a user to the database. Contact your technical advisor to review the logs and rejected users."));
        }

        [Test]
        public void GetErrorMessage_WhenFailedParsing_ReturnsFailedParsingMessage()
        {
            var failedUserData = new FailedUserData(0, "userdata", RejectionReason.FailedParsing);
            string actual = systemUnderTest.GetErrorMessage(failedUserData);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem parsing a user from the stats file. The user 'userdata' failed data parsing. You should contact your technical advisor to review the logs and rejected users."));
        }

        [Test]
        public void GetErrorMessage_WhenFailedUsersData_ReturnsFailedUserDataMessage()
        {
            var failedUsersData = new List<FailedUserData> { new FailedUserData(), new FailedUserData() };
            string actual = systemUnderTest.GetErrorMessage(failedUsersData);

            Assert.That(actual,
                Is.EqualTo(
                    $"There was a problem uploading the file payload. The file passed validation but {failedUsersData.Count} lines failed; processing continued after encountering these lines. If this problem occurs again, then you should contact your technical advisor to review the logs and failed users."));
        }

        [Test]
        public void GetErrorMessage_WhenFileDownloadFailedDecompression_ReturnsFileDownloadFailedDecompressionMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.FileDownloadFailedDecompression,
                new FilePayload(), StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem decompressing the file payload. The file has been moved to a failed directory for review. If this problem occurs again, then you should contact your technical advisor to review the logs and failed files."));
        }

        [Test]
        public void GetErrorMessage_WhenFileDownloadTimeout_ReturnsFileDownloadTimeoutMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.FileDownloadTimeout, new FilePayload(),
                StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. There was a timeout when downloading the file payload. If a timeout occurs again, then you can try increasing the configurable download timeout."));
        }

        [Test]
        public void GetErrorMessage_WhenInvalidStatsFile_ReturnsInvalidStatsFileMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.InvalidStatsFileUpload,
                StatsDownloadService.StatsUpload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem uploading the file payload. The file failed validation; check the logs for more information. If this problem occurs again, then you should contact your technical advisor to review the logs and failed uploads."));
        }

        [Test]
        public void GetErrorMessage_WhenMinimumWaitTimeNotMet_ReturnsMinimumWaitTimeNotMetMessage()
        {
            var configuredWaitTime = new TimeSpan(1, 0, 0, 0);

            string actual = systemUnderTest.GetErrorMessage(FailedReason.MinimumWaitTimeNotMet,
                new FilePayload { MinimumWaitTimeSpan = configuredWaitTime }, StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    $"There was a problem downloading the file payload. The file download service was run before the minimum wait time {MinimumWait.TimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again."));
        }

        [Test]
        public void GetErrorMessage_WhenNoFailedReason_ReturnsEmptyMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.None, new FilePayload(),
                StatsDownloadService.FileDownload);

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetErrorMessage_WhenRequiredSettingsInvalid_ReturnsRequiredSettingsInvalidMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.RequiredSettingsInvalid, new FilePayload(),
                StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. The required settings are invalid; check the logs for more information. Ensure the settings are complete and accurate, then try again."));
        }

        [Test]
        public void GetErrorMessage_WhenStatsUploadTimeout_ReturnsStatsUploadTimeoutMessage()
        {
            string actual =
                systemUnderTest.GetErrorMessage(FailedReason.StatsUploadTimeout, StatsDownloadService.StatsUpload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem uploading the file payload. There was a timeout when uploading the stats and the file has been marked rejected. If a timeout occurs again, then you can try increasing the configurable command timeout."));
        }

        [Test]
        public void
            GetErrorMessage_WhenUnexpectedExceptionDuringFileDownload_ReturnsFileDownloadUnexpectedExceptionMessage()
        {
            string actual = systemUnderTest.GetErrorMessage(FailedReason.UnexpectedException, new FilePayload(),
                StatsDownloadService.FileDownload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem downloading the file payload. There was an unexpected exception. Check the log for more information."));
        }

        [Test]
        public void
            GetErrorMessage_WhenUnexpectedExceptionDuringStatsUpload_ReturnsStatsUploadUnexpectedExceptionMessage()
        {
            string actual =
                systemUnderTest.GetErrorMessage(FailedReason.UnexpectedException, StatsDownloadService.StatsUpload);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem uploading the file payload. There was an unexpected exception. Check the log for more information."));
        }

        [Test]
        public void GetErrorMessage_WhenUnexpectedFormat_ReturnsUnexpectedFormatMessage()
        {
            var failedUserData = new FailedUserData(0, "userdata", RejectionReason.UnexpectedFormat);
            string actual = systemUnderTest.GetErrorMessage(failedUserData);

            Assert.That(actual,
                Is.EqualTo(
                    "There was a problem parsing a user from the stats file. The user 'userdata' was in an unexpected format. You should contact your technical advisor to review the logs and rejected users."));
        }

        [Test]
        public void GetErrorMessage_WhenUnknownRejectionReason_ReturnsEmptyString()
        {
            string actual = systemUnderTest.GetErrorMessage(new FailedUserData(0, string.Empty,
                (RejectionReason) Enum.Parse(typeof (RejectionReason), "-1")));

            Assert.IsEmpty(actual);
        }
    }
}