namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Email;

    public class StatsDownloadEmailProvider : IStatsDownloadEmailService
    {
        // TODO: Move these into a localizable resource
        private const string FileDownloadFailedSubject = "File Download Failed";

        private const string StatsUploadFailedSubject = "Stats Upload Failed";

        private const string UserDataFailedParsingSubject = "User Data Failed";

        private readonly IEmailService emailService;

        private readonly EmailSettings emailSettings;

        private readonly IErrorMessageService errorMessageService;

        public StatsDownloadEmailProvider(IEmailService emailService, IErrorMessageService errorMessageService,
                                          IOptions<EmailSettings> emailSettings)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.errorMessageService =
                errorMessageService ?? throw new ArgumentNullException(nameof(errorMessageService));
            this.emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
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
            SendEmail("Test Email", "This is a test email.");
        }

        private string GetSubject(string baseSubject)
        {
            string displayName = emailSettings.DisplayName;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                return baseSubject;
            }

            return $"{displayName} - {baseSubject}";
        }

        private void SendEmail(string baseSubject, string body)
        {
            string subject = GetSubject(baseSubject);

            emailService.SendEmail(subject, body);
        }
    }
}