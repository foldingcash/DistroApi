namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.IO;
    using Interfaces.DataTransfer;

    using StatsDownload.Core.DataTransfer;
    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Logging;

    public class FilePayloadSettingsProvider : IFilePayloadSettingsService
    {
        private const string DecompressedFileExtension = ".txt";

        private const string DecompressedFileName = "daily_user_summary";

        private const string FileExtension = ".bz2";

        private const string FileName = "daily_user_summary.txt";

        private readonly IDateTimeService dateTimeService;

        private readonly IDownloadSettingsService downloadSettingsService;

        private readonly IDownloadSettingsValidatorService downloadSettingsValidatorService;

        private readonly ILoggingService loggingService;

        public FilePayloadSettingsProvider(IDateTimeService dateTimeService,
                                           IDownloadSettingsService downloadSettingsService,
                                           IDownloadSettingsValidatorService downloadSettingsValidatorService,
                                           ILoggingService loggingService)
        {
            if (dateTimeService == null)
            {
                throw new ArgumentNullException(nameof(dateTimeService));
            }

            if (downloadSettingsService == null)
            {
                throw new ArgumentNullException(nameof(downloadSettingsService));
            }

            if (downloadSettingsValidatorService == null)
            {
                throw new ArgumentNullException(nameof(downloadSettingsValidatorService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            this.dateTimeService = dateTimeService;
            this.downloadSettingsService = downloadSettingsService;
            this.downloadSettingsValidatorService = downloadSettingsValidatorService;
            this.loggingService = loggingService;
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
            string downloadDirectory = downloadSettingsService.GetDownloadDirectory();

            if (!downloadSettingsValidatorService.IsValidDownloadDirectory(downloadDirectory))
            {
                throw NewFileDownloadArgumentException("Download directory is invalid");
            }

            return downloadDirectory;
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

        private Exception NewFileDownloadArgumentException(string message)
        {
            return new FileDownloadArgumentException(message);
        }

        private void SetDecompressedDownloadFileDetails(FilePayload filePayload, DateTime dateTime,
                                                        string downloadDirectory)
        {
            string decompressedFileName = $"{dateTime.ToFileTime()}.{DecompressedFileName}";

            filePayload.DecompressedDownloadDirectory = downloadDirectory;
            filePayload.DecompressedDownloadFileName = decompressedFileName;
            filePayload.DecompressedDownloadFileExtension = DecompressedFileExtension;
            filePayload.DecompressedDownloadFilePath = Path.Combine(downloadDirectory,
                $"{decompressedFileName}{DecompressedFileExtension}");
        }

        private void SetDownloadDetails(FilePayload filePayload)
        {
            string downloadTimeout = GetDownloadTimeout();
            string unsafeDownloadUri = GetDownloadUri();
            string unsafeAcceptAnySslCert = GetAcceptAnySslCert();
            string unsafeMinimumWaitTimeInHours = GetMinimumWaitTimeInHours();

            Uri downloadUri;
            if (!TryParseDownloadUri(unsafeDownloadUri, out downloadUri))
            {
                throw NewFileDownloadArgumentException("Download Uri is invalid");
            }

            int timeoutInSeconds;
            if (!TryParseTimeout(downloadTimeout, out timeoutInSeconds))
            {
                timeoutInSeconds = 100;
                loggingService.LogVerbose("The download timeout configuration was invalid, using the default value.");
            }

            bool acceptAnySslCert;
            if (!TryParseAcceptAnySslCert(unsafeAcceptAnySslCert, out acceptAnySslCert))
            {
                acceptAnySslCert = false;
                loggingService.LogVerbose("The accept any SSL cert configuration was invalid, using the default value.");
            }

            TimeSpan minimumWaitTimeSpan;
            if (!TryParseMinimumWaitTimeSpan(unsafeMinimumWaitTimeInHours, out minimumWaitTimeSpan))
            {
                minimumWaitTimeSpan = MinimumWait.TimeSpan;
                loggingService.LogVerbose("The minimum wait time configuration was invalid, using the default value.");
            }

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
            return downloadSettingsValidatorService.TryParseAcceptAnySslCert(unsafeAcceptAnySslCert,
                out acceptAnySslCert);
        }

        private bool TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri)
        {
            return downloadSettingsValidatorService.TryParseDownloadUri(unsafeDownloadUri, out downloadUri);
        }

        private bool TryParseMinimumWaitTimeSpan(string unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan)
        {
            return downloadSettingsValidatorService.TryParseMinimumWaitTimeSpan(unsafeMinimumWaitTimeInHours,
                out minimumWaitTimeSpan);
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return downloadSettingsValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }
    }
}