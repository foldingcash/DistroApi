namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FilePayloadSettingsProvider : IFilePayloadSettingsService
    {
        private const string FileExtension = ".bz2";

        private const string FileName = "daily_user_summary.txt";

        private const string UncompressedFileExtension = ".txt";

        private const string UncompressedFileName = "daily_user_summary";

        private readonly IDateTimeService dateTimeService;

        private readonly IFileDownloadSettingsService fileDownloadSettingsService;

        private readonly IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService;

        public FilePayloadSettingsProvider(
            IDateTimeService dateTimeService,
            IFileDownloadSettingsService fileDownloadSettingsService,
            IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService)
        {
            if (IsNull(dateTimeService))
            {
                throw NewArgumentNullException(nameof(dateTimeService));
            }

            if (IsNull(fileDownloadSettingsService))
            {
                throw NewArgumentNullException(nameof(fileDownloadSettingsService));
            }

            if (IsNull(fileDownloadTimeoutValidatorService))
            {
                throw NewArgumentNullException(nameof(fileDownloadTimeoutValidatorService));
            }

            this.dateTimeService = dateTimeService;
            this.fileDownloadSettingsService = fileDownloadSettingsService;
            this.fileDownloadTimeoutValidatorService = fileDownloadTimeoutValidatorService;
        }

        public void SetFilePayloadDownloadDetails(FilePayload filePayload)
        {
            DateTime now = DateTimeNow();
            string downloadDirectory = GetDownloadDirectory();

            SetDownloadDetails(filePayload);
            SetDownloadFileDetails(filePayload, now, downloadDirectory);
            SetUncompressedDownloadFileDetails(filePayload, now, downloadDirectory);
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }

        private string GetDownloadDirectory()
        {
            return fileDownloadSettingsService.GetDownloadDirectory();
        }

        private string GetDownloadTimeout()
        {
            return fileDownloadSettingsService.GetDownloadTimeout();
        }

        private string GetDownloadUri()
        {
            return fileDownloadSettingsService.GetDownloadUri();
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private void SetDownloadDetails(FilePayload filePayload)
        {
            string downloadTimeout = GetDownloadTimeout();
            string downloadUri = GetDownloadUri();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            filePayload.DownloadUri = new Uri(downloadUri);
            filePayload.TimeoutSeconds = timeoutInSeconds;
        }

        private void SetDownloadFileDetails(FilePayload filePayload, DateTime dateTime, string downloadDirectory)
        {
            string fileName = $"{dateTime.ToFileTime()}.{FileName}";

            filePayload.DownloadDirectory = downloadDirectory;
            filePayload.DownloadFileName = fileName;
            filePayload.DownloadFileExtension = FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, $"{fileName}{FileExtension}");
        }

        private void SetUncompressedDownloadFileDetails(
            FilePayload filePayload,
            DateTime dateTime,
            string downloadDirectory)
        {
            string uncompressedFileName = $"{dateTime.ToFileTime()}.{UncompressedFileName}";

            filePayload.UncompressedDownloadDirectory = downloadDirectory;
            filePayload.UncompressedDownloadFileName = uncompressedFileName;
            filePayload.UncompressedDownloadFileExtension = UncompressedFileExtension;
            filePayload.UncompressedDownloadFilePath = Path.Combine(
                downloadDirectory,
                $"{uncompressedFileName}{UncompressedFileExtension}");
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return fileDownloadTimeoutValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }
    }
}