namespace StatsDownload.Core
{
    using StatsDownload.Email;

    public class FileDownloadEmailProvider : IFileDownloadEmailService
    {
        private const string FileDownloadFailSubject = "File Download Failed";

        private readonly IEmailService emailService;

        private readonly IErrorMessageService errorMessageService;

        public FileDownloadEmailProvider(IEmailService emailService, IErrorMessageService errorMessageService)
        {
            this.emailService = emailService;
            this.errorMessageService = errorMessageService;
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            FailedReason failedReason = fileDownloadResult.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason, fileDownloadResult.FilePayload);

            SendEmail(FileDownloadFailSubject, errorMessage);
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}