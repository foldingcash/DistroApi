namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using Email;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;

    public class StatsDownloadEmailProvider : IStatsDownloadEmailService
    {
        // TODO: Move these into a localizable resource
        private const string FileDownloadFailedSubject = "File Download Failed";

        private const string StatsUploadFailedSubject = "Stats Upload Failed";

        private const string UserDataFailedParsingSubject = "User Data Failed";

        private readonly IEmailService emailService;

        private readonly IErrorMessageService errorMessageService;

        public StatsDownloadEmailProvider(IEmailService emailService, IErrorMessageService errorMessageService)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.errorMessageService =
                errorMessageService ?? throw new ArgumentNullException(nameof(errorMessageService));
        }

        public void SendEmail(FileDownloadResult fileDownloadResult)
        {
            FailedReason failedReason = fileDownloadResult.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason, fileDownloadResult.FilePayload,
                StatsDownloadService.FileDownload);

            SendEmail(FileDownloadFailedSubject, errorMessage);
        }

        public void SendEmail(StatsUploadResult statsUploadResult)
        {
            FailedReason failedReason = statsUploadResult.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason, StatsDownloadService.StatsUpload);

            SendEmail(StatsUploadFailedSubject, errorMessage);
        }

        public void SendEmail(StatsUploadResults statsUploadResults)
        {
            FailedReason failedReason = statsUploadResults.FailedReason;

            string errorMessage = errorMessageService.GetErrorMessage(failedReason, StatsDownloadService.StatsUpload);

            SendEmail(StatsUploadFailedSubject, errorMessage);
        }

        public void SendEmail(IEnumerable<FailedUserData> failedUsersData)
        {
            string errorMessage = errorMessageService.GetErrorMessage(failedUsersData);

            SendEmail(UserDataFailedParsingSubject, errorMessage);
        }

        public void SendTestEmail()
        {
            SendEmail("TestHarness - Test Email", "This is a test email from the TestHarness.");
        }

        private void SendEmail(string subject, string body)
        {
            emailService.SendEmail(subject, body);
        }
    }
}