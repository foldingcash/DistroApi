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

        private readonly IDownloadSettingsService downloadSettingsService;

        private readonly IDownloadSettingsValidatorService downloadSettingsValidatorService;

        public FilePayloadSettingsProvider(
            IDateTimeService dateTimeService,
            IDownloadSettingsService downloadSettingsService,
            IDownloadSettingsValidatorService downloadSettingsValidatorService)
        {
            if (IsNull(dateTimeService))
            {
                throw NewArgumentNullException(nameof(dateTimeService));
            }

            if (IsNull(downloadSettingsService))
            {
                throw NewArgumentNullException(nameof(downloadSettingsService));
            }

            if (IsNull(downloadSettingsValidatorService))
            {
                throw NewArgumentNullException(nameof(downloadSettingsValidatorService));
            }

            this.dateTimeService = dateTimeService;
            this.downloadSettingsService = downloadSettingsService;
            this.downloadSettingsValidatorService = downloadSettingsValidatorService;
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

        private string GetAcceptAnySslCert()
        {
            return downloadSettingsService.GetAcceptAnySslCert();
        }

        private string GetDownloadDirectory()
        {
            return downloadSettingsService.GetDownloadDirectory();
        }

        private string GetDownloadTimeout()
        {
            return downloadSettingsService.GetDownloadTimeout();
        }

        private string GetDownloadUri()
        {
            return downloadSettingsService.GetDownloadUri();
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
            string unsafeAcceptAnySslCert = GetAcceptAnySslCert();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            bool acceptAnySslCert;
            TryParseAcceptAnySslCert(unsafeAcceptAnySslCert, out acceptAnySslCert);

            filePayload.DownloadUri = new Uri(downloadUri);
            filePayload.TimeoutSeconds = timeoutInSeconds;
            filePayload.AcceptAnySslCert = acceptAnySslCert;
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

        private bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return downloadSettingsValidatorService.TryParseAcceptAnySslCert(
                unsafeAcceptAnySslCert,
                out acceptAnySslCert);
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return downloadSettingsValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }
    }
}