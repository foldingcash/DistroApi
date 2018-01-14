namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FilePayloadSettingsProvider : IFilePayloadSettingsService
    {
        private const string DecompressedFileExtension = ".txt";

        private const string DecompressedFileName = "daily_user_summary";

        private const string FileExtension = ".bz2";

        private const string FileName = "daily_user_summary.txt";

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
            SetDecompressedDownloadFileDetails(filePayload, now, downloadDirectory);
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

        private string GetMinimumWaitTimeInHours()
        {
            return downloadSettingsService.GetMinimumWaitTimeInHours();
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private void SetDecompressedDownloadFileDetails(
            FilePayload filePayload,
            DateTime dateTime,
            string downloadDirectory)
        {
            string decompressedFileName = $"{dateTime.ToFileTime()}.{DecompressedFileName}";

            filePayload.DecompressedDownloadDirectory = downloadDirectory;
            filePayload.DecompressedDownloadFileName = decompressedFileName;
            filePayload.DecompressedDownloadFileExtension = DecompressedFileExtension;
            filePayload.DecompressedDownloadFilePath = Path.Combine(
                downloadDirectory,
                $"{decompressedFileName}{DecompressedFileExtension}");
        }

        private void SetDownloadDetails(FilePayload filePayload)
        {
            string downloadTimeout = GetDownloadTimeout();
            string downloadUri = GetDownloadUri();
            string unsafeAcceptAnySslCert = GetAcceptAnySslCert();
            string unsafeMinimumWaitTimeInHours = GetMinimumWaitTimeInHours();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            bool acceptAnySslCert;
            TryParseAcceptAnySslCert(unsafeAcceptAnySslCert, out acceptAnySslCert);

            TimeSpan minimumWaitTimeSpan;
            TryParseMinimumWaitTimeSpan(unsafeMinimumWaitTimeInHours, out minimumWaitTimeSpan);

            filePayload.DownloadUri = new Uri(downloadUri);
            filePayload.TimeoutSeconds = timeoutInSeconds;
            filePayload.AcceptAnySslCert = acceptAnySslCert;
            filePayload.MinimumWaitTimeSpan = minimumWaitTimeSpan;
        }

        private void SetDownloadFileDetails(FilePayload filePayload, DateTime dateTime, string downloadDirectory)
        {
            string fileName = $"{dateTime.ToFileTime()}.{FileName}";

            filePayload.DownloadDirectory = downloadDirectory;
            filePayload.DownloadFileName = fileName;
            filePayload.DownloadFileExtension = FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, $"{fileName}{FileExtension}");
        }

        private bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return downloadSettingsValidatorService.TryParseAcceptAnySslCert(
                unsafeAcceptAnySslCert,
                out acceptAnySslCert);
        }

        private void TryParseMinimumWaitTimeSpan(string unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan)
        {
            downloadSettingsValidatorService.TryParseMinimumWaitTimeSpan(
                unsafeMinimumWaitTimeInHours,
                out minimumWaitTimeSpan);
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return downloadSettingsValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }
    }
}