namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;

    public class ErrorMessageProvider : IErrorMessageService
    {
        private const string FileDownloadFailBodyStart = "There was a problem downloading the file payload.";

        private const string FileDownloadFailDecompressionBodyStart =
            "There was a problem decompressing the file payload.";

        private const string StatsUploadFailBodyStart = "There was a problem uploading the file payload.";

        public string GetErrorMessage(FailedReason failedReason, FilePayload filePayload)
        {
            if (failedReason == FailedReason.MinimumWaitTimeNotMet)
            {
                TimeSpan minimumWaitTimeSpan = MinimumWait.TimeSpan;
                TimeSpan configuredWaitTime = filePayload.MinimumWaitTimeSpan;
                return FileDownloadFailBodyStart
                       + $" The file download service was run before the minimum wait time {minimumWaitTimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again.";
            }

            return GetErrorMessage(failedReason);
        }

        public string GetErrorMessage(FailedReason failedReason)
        {
            if (failedReason == FailedReason.DataStoreUnavailable)
            {
                return FileDownloadFailBodyStart
                       + " The data store is unavailable, ensure the data store is available and configured correctly and try again.";
            }
            if (failedReason == FailedReason.RequiredSettingsInvalid)
            {
                return FileDownloadFailBodyStart
                       + " The required settings are invalid; check the logs for more information. Ensure the settings are complete and accurate, then try again.";
            }
            if (failedReason == FailedReason.FileDownloadTimeout)
            {
                return FileDownloadFailBodyStart
                       + " There was a timeout when downloading the file payload. If a timeout occurs again, then you can try increasing the configurable download timeout.";
            }
            if (failedReason == FailedReason.FileDownloadFailedDecompression)
            {
                return FileDownloadFailDecompressionBodyStart
                       + " The file has been moved to a failed directory for review. If this problem occurs again, then you should contact your technical advisor to review the logs and failed files.";
            }
            if (failedReason == FailedReason.InvalidStatsFileUpload)
            {
                return StatsUploadFailBodyStart
                       + " The file failed validation; check the logs for more information. If this problem occurs again, then you should contact your technical advisor to review the logs and failed uploads.";
            }
            if (failedReason == FailedReason.UnexpectedException)
            {
                return FileDownloadFailBodyStart + " Check the log for more information.";
            }
            return string.Empty;
        }

        public string GetErrorMessage(List<FailedUserData> failedUsersData)
        {
            return
                $"There was a problem uploading the file payload. The file passed validation but {failedUsersData.Count} lines failed validation; processing continued after encountering these lines. If this problem occurs again, then you should contact your technical advisor to review the logs and failed user parsings.";
        }
    }
}