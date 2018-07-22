namespace StatsDownload.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using Core.Interfaces.Enums;

    public class ErrorMessageProvider : IErrorMessageService
    {
        public string GetErrorMessage(FailedReason failedReason, FilePayload filePayload, StatsDownloadService service)
        {
            if (failedReason == FailedReason.MinimumWaitTimeNotMet)
            {
                TimeSpan minimumWaitTimeSpan = MinimumWait.TimeSpan;
                TimeSpan configuredWaitTime = filePayload.MinimumWaitTimeSpan;
                return String.Format(ErrorMessages.MinimumWaitTimeNotMet, minimumWaitTimeSpan, configuredWaitTime);
            }

            return GetErrorMessage(failedReason, service);
        }

        public string GetErrorMessage(FailedReason failedReason, StatsDownloadService service)
        {
            if (failedReason == FailedReason.DatabaseUnavailable)
            {
                return GetErrorMessageByServiceType(service, ErrorMessages.FileDownloadDatabaseUnavailable,
                    ErrorMessages.StatsUploadDatabaseUnavailable, ErrorMessages.DefaultDatabaseUnavailable);
            }

            if (failedReason == FailedReason.RequiredSettingsInvalid)
            {
                return ErrorMessages.RequiredSettingsAreInvalid;
            }

            if (failedReason == FailedReason.FileDownloadTimeout)
            {
                return ErrorMessages.FileDownloadTimedOut;
            }

            if (failedReason == FailedReason.FileDownloadFailedDecompression)
            {
                return ErrorMessages.FileDownloadFailedDecompression;
            }

            if (failedReason == FailedReason.InvalidStatsFileUpload)
            {
                return ErrorMessages.InvalidStatsFile;
            }

            if (failedReason == FailedReason.UnexpectedDatabaseException)
            {
                return ErrorMessages.StatsUploadTimeout;
            }

            if (failedReason == FailedReason.UnexpectedException)
            {
                return GetErrorMessageByServiceType(service, ErrorMessages.FileDownloadUnexpectedException,
                    ErrorMessages.StatsUploadUnexpectedException, ErrorMessages.DefaultUnexpectedException) ;
            }

            return string.Empty;
        }

        public string GetErrorMessage(IEnumerable<FailedUserData> failedUsersData)
        {
            return string.Format(ErrorMessages.FailedUserDataCount, failedUsersData.Count());
        }

        public string GetErrorMessage(FailedUserData failedUserData)
        {
            RejectionReason rejectionReason = failedUserData.RejectionReason;
            string data = failedUserData.Data;

            if (rejectionReason == RejectionReason.FailedParsing)
            {
                return ErrorMessages.FailedParsingUserData;
            }

            if (rejectionReason == RejectionReason.FailedAddToDatabase)
            {
                return ErrorMessages.FailedAddUserToDatabase;
            }

            if (rejectionReason == RejectionReason.UnexpectedFormat)
            {
                return ErrorMessages.UserDataUnexpectedFormat;
            }

            return string.Empty;
        }

        private string GetErrorMessageByServiceType(StatsDownloadService service, string fileDownloadMessage, string statsUploadMessage, string defaultMessage)
        {
            if (service == StatsDownloadService.FileDownload)
            {
                return fileDownloadMessage;
            }

            if (service == StatsDownloadService.StatsUpload)
            {
                return statsUploadMessage;
            }

            return defaultMessage;
        }
    }
}