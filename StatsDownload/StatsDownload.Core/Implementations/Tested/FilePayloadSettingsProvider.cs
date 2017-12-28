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
            this.dateTimeService = dateTimeService;
            this.fileDownloadSettingsService = fileDownloadSettingsService;
            this.fileDownloadTimeoutValidatorService = fileDownloadTimeoutValidatorService;
        }

        public void SetFilePayloadDownloadDetails(FilePayload filePayload)
        {
            string downloadDirectory = GetDownloadDirectory();

            DateTime now = DateTimeNow();

            string fileName = $"{now.ToFileTime()}.{FileName}";
            string uncompressedFileName = $"{now.ToFileTime()}.{UncompressedFileName}";

            filePayload.DownloadDirectory = downloadDirectory;
            filePayload.DownloadFileName = fileName;
            filePayload.DownloadFileExtension = FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, $"{fileName}{FileExtension}");

            filePayload.UncompressedDownloadDirectory = downloadDirectory;
            filePayload.UncompressedDownloadFileName = uncompressedFileName;
            filePayload.UncompressedDownloadFileExtension = UncompressedFileExtension;
            filePayload.UncompressedDownloadFilePath = Path.Combine(
                downloadDirectory,
                $"{uncompressedFileName}{UncompressedFileExtension}");

            string downloadTimeout = GetDownloadTimeout();
            string downloadUri = GetDownloadUri();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            filePayload.DownloadUri = new Uri(downloadUri);
            filePayload.TimeoutSeconds = timeoutInSeconds;
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

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return fileDownloadTimeoutValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }
    }
}