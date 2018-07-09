﻿namespace StatsDownload.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;

    public class ErrorMessageProvider : IErrorMessageService
    {
        // TODO: Move these strings into a localizable resource
        private const string FileDownloadFailBodyStart = "There was a problem downloading the file payload.";

        private const string FileDownloadFailDecompressionBodyStart =
            "There was a problem decompressing the file payload.";

        private const string StatsUploadFailBodyStart = "There was a problem uploading the file payload.";

        public string GetErrorMessage(FailedReason failedReason, FilePayload filePayload, StatsDownloadService service)
        {
            if (failedReason == FailedReason.MinimumWaitTimeNotMet)
            {
                TimeSpan minimumWaitTimeSpan = MinimumWait.TimeSpan;
                TimeSpan configuredWaitTime = filePayload.MinimumWaitTimeSpan;
                return FileDownloadFailBodyStart
                       + $" The file download service was run before the minimum wait time {minimumWaitTimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again.";
            }

            return GetErrorMessage(failedReason, service);
        }

        public string GetErrorMessage(FailedReason failedReason, StatsDownloadService service)
        {
            if (failedReason == FailedReason.DataStoreUnavailable)
            {
                return GetBodyStart(service) +
                       "There was a problem connecting to the data store. The data store is unavailable, ensure the data store is available and configured correctly and try again.";
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

            if (failedReason == FailedReason.UnexpectedDatabaseException)
            {
                return StatsUploadFailBodyStart +
                       " There was an unexpected database exception and the file has been marked rejected. If this problem occurs again, then you should contact your technical advisor to review the rejections and logs.";
            }

            if (failedReason == FailedReason.UnexpectedException)
            {
                return GetBodyStart(service) +
                       "There was an unexpected exception. Check the log for more information.";
            }

            return string.Empty;
        }

        public string GetErrorMessage(IEnumerable<FailedUserData> failedUsersData)
        {
            return
                $"There was a problem uploading the file payload. The file passed validation but {failedUsersData.Count()} lines failed; processing continued after encountering these lines. If this problem occurs again, then you should contact your technical advisor to review the logs and failed users.";
        }

        public string GetErrorMessage(FailedUserData failedUserData)
        {
            RejectionReason rejectionReason = failedUserData.RejectionReason;
            string data = failedUserData.Data;

            if (rejectionReason == RejectionReason.FailedParsing)
            {
                return
                    $"There was a problem parsing a user from the stats file. The user '{data}' failed data parsing. You should contact your technical advisor to review the logs and rejected users.";
            }

            if (rejectionReason == RejectionReason.FailedAddToDatabase)
            {
                return
                    "There was a problem adding a user to the database. Contact your technical advisor to review the logs and rejected users.";
            }

            if (rejectionReason == RejectionReason.UnexpectedFormat)
            {
                return
                    $"There was a problem parsing a user from the stats file. The user '{data}' was in an unexpected format. You should contact your technical advisor to review the logs and rejected users.";
            }

            return string.Empty;
        }

        private string GetBodyStart(StatsDownloadService service)
        {
            if (service == StatsDownloadService.FileDownload)
            {
                return $"{FileDownloadFailBodyStart} ";
            }

            if (service == StatsDownloadService.StatsUpload)
            {
                return $"{StatsUploadFailBodyStart} ";
            }

            return string.Empty;
        }
    }
}