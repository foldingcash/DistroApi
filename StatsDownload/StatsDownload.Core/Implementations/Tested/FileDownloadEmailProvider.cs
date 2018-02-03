namespace StatsDownload.Core
{
    using System;

    using StatsDownload.Email;

    public class FileDownloadEmailProvider : IFileDownloadEmailService
    {
        private const string FileDownloadFailBodyStart = "There was a problem downloading the file payload.";

        private const string FileDownloadFailSubject = "File Download Failed";

        private readonly IEmailService emailService;

        public FileDownloadEmailProvider(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            if (fileDownloadResult.FailedReason == FailedReason.DataStoreUnavailable)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + " The data store is unavailable, ensure the data store is available and configured correctly and try again.");
            }

            else if (fileDownloadResult.FailedReason == FailedReason.MinimumWaitTimeNotMet)
            {
                TimeSpan minimumWaitTimeSpan = MinimumWait.TimeSpan;
                TimeSpan configuredWaitTime = fileDownloadResult.FilePayload.MinimumWaitTimeSpan;
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + $" The file download service was run before the minimum wait time {minimumWaitTimeSpan} or the configured wait time {configuredWaitTime}. Configure to run the service less often or decrease your configured wait time and try again.");
            }
            else if (fileDownloadResult.FailedReason == FailedReason.FileDownloadTimeout)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    FileDownloadFailBodyStart
                    + " There was a timeout when downloading the file payload. If a timeout occurs again when trying to download the file payload, then you can try increasing the download timeout.");
            }
            else if (fileDownloadResult.FailedReason == FailedReason.UnexpectedException)
            {
                SendEmail(
                    FileDownloadFailSubject,
                    "There was a catastrophic problem downloading the file payload. Check the log for more information.");
            }
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}