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
            SetFailedDownloadFileDetails(filePayload, now, downloadDirectory);
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

        private string GetDownloadFileName(DateTime dateTime)
        {
            return $"{dateTime.ToFileTime()}.{FileName}";
        }

        private string GetDownloadFileNameWithExtension(DateTime dateTime)
        {
            return $"{dateTime.ToFileTime()}.{FileName}{FileExtension}";
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
            string unsafeDownloadUri = GetDownloadUri();
            string unsafeAcceptAnySslCert = GetAcceptAnySslCert();
            string unsafeMinimumWaitTimeInHours = GetMinimumWaitTimeInHours();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            bool acceptAnySslCert;
            TryParseAcceptAnySslCert(unsafeAcceptAnySslCert, out acceptAnySslCert);

            TimeSpan minimumWaitTimeSpan;
            TryParseMinimumWaitTimeSpan(unsafeMinimumWaitTimeInHours, out minimumWaitTimeSpan);

            Uri downloadUri;
            TryParseDownloadUri(unsafeDownloadUri, out downloadUri);

            filePayload.DownloadUri = downloadUri;
            filePayload.TimeoutSeconds = timeoutInSeconds;
            filePayload.AcceptAnySslCert = acceptAnySslCert;
            filePayload.MinimumWaitTimeSpan = minimumWaitTimeSpan;
        }

        private void SetDownloadFileDetails(FilePayload filePayload, DateTime dateTime, string downloadDirectory)
        {
            filePayload.DownloadDirectory = downloadDirectory;
            filePayload.DownloadFileName = GetDownloadFileName(dateTime);
            filePayload.DownloadFileExtension = FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, GetDownloadFileNameWithExtension(dateTime));
        }

        private void SetFailedDownloadFileDetails(FilePayload filePayload, DateTime dateTime, string downloadDirectory)
        {
            string downloadFileName = GetDownloadFileNameWithExtension(dateTime);

            filePayload.FailedDownloadFilePath = Path.Combine(downloadDirectory, "FileDownloadFailed", downloadFileName);
        }

        private bool TryParseAcceptAnySslCert(string unsafeAcceptAnySslCert, out bool acceptAnySslCert)
        {
            return downloadSettingsValidatorService.TryParseAcceptAnySslCert(
                unsafeAcceptAnySslCert,
                out acceptAnySslCert);
        }

        private void TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri)
        {
            downloadSettingsValidatorService.TryParseDownloadUri(unsafeDownloadUri, out downloadUri);
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