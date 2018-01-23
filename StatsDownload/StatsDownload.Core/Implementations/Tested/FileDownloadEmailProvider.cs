namespace StatsDownload.Core
{
    using System;

    using StatsDownload.Email;

    public class FileDownloadEmailProvider : IFileDownloadEmailService
    {
        private const string FileDownloadFailBodyStart = "There was a problem downloading the file payload.";

        private const string FileDownloadFailDecompressionBodyStart =
            "There was a problem decompressing the file payload.";

        private const string FileDownloadFailSubject = "File Download Failed";

        private readonly IEmailService emailService;

        public FileDownloadEmailProvider(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            FailedReason failedReason = fileDownloadResult.FailedReason;

            if (failedReason == FailedReason.DataStoreUnavailable)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + " The data store is unavailable, ensure the data store is available and configured correctly and try again.");
            }
            else if (failedReason == FailedReason.RequiredSettingsInvalid)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + " The required settings are invalid; check the logs for more information. Ensure the settings are complete and accurate, then try again.");
            }
            else if (failedReason == FailedReason.MinimumWaitTimeNotMet)
            {
                TimeSpan minimumWaitTimeSpan = MinimumWait.TimeSpan;
                TimeSpan configuredWaitTime = fileDownloadResult.FilePayload.MinimumWaitTimeSpan;
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + $" The file download service was run before the minimum wait time {minimumWaitTimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again.");
            }
            else if (failedReason == FailedReason.FileDownloadTimeout)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + " There was a timeout when downloading the file payload. If a timeout occurs again, then you can try increasing the configurable download timeout.");
            }
            else if (failedReason == FailedReason.FileDownloadFailedDecompression)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailDecompressionBodyStart
                    + " The file has been moved to a failed directory for review. If this problem occurs again, then you should contact your technical advisor to review the logs and failed files.");
            }
            else if (failedReason == FailedReason.UnexpectedException)
            {
                SendEmail(FileDownloadFailSubject, FileDownloadFailBodyStart + " Check the log for more information.");
            }
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}