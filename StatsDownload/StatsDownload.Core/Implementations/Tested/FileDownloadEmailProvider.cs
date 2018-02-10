namespace StatsDownload.Core
{
    using StatsDownload.Email;

    public class FileDownloadEmailProvider : IFileDownloadEmailService
    {
        private const string FileDownloadFailBodyStart = "There was a problem downloading the file payload.";

        private const string FileDownloadFailDecompressionBodyStart =
            "There was a problem decompressing the file payload.";

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