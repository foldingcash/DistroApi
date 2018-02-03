namespace StatsDownload.Core
{
    using StatsDownload.Email;

    public class FileDownloadEmailProvider : IFileDownloadEmailService
    {
        private readonly IEmailService emailService;

        public FileDownloadEmailProvider(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            SendEmail("File Download Failed", "");
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}