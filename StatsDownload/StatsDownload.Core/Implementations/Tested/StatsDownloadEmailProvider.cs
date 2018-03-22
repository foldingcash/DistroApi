namespace StatsDownload.Core
{
    using StatsDownload.Email;

    public class StatsDownloadEmailProvider : IStatsDownloadEmailService
    {
        private const string FileDownloadFailedSubject = "File Download Failed";

        private const string StatsUploadFailedSubject = "Stats Upload Failed";

        private readonly IEmailService emailService;

        private readonly IErrorMessageService errorMessageService;

        public StatsDownloadEmailProvider(IEmailService emailService, IErrorMessageService errorMessageService)
        {
            this.emailService = emailService;
            this.errorMessageService = errorMessageService;
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            FailedReason failedReason = fileDownloadResult.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason, fileDownloadResult.FilePayload);

            SendEmail(FileDownloadFailedSubject, errorMessage);
        }

        public void SendEmail(StatsUploadResult statsUploadResult)
        {
            FailedReason failedReason = statsUploadResult.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason);

            SendEmail(StatsUploadFailedSubject, errorMessage);
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}