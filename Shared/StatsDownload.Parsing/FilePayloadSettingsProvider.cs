namespace StatsDownload.Parsing
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Settings;

    public class FilePayloadSettingsProvider : IFilePayloadSettingsService
    {
        private readonly DataStoreSettings dataStoreSettings;

        private readonly IDateTimeService dateTimeService;

        private readonly DownloadSettings downloadSettings;

        private readonly IDownloadSettingsValidatorService downloadSettingsValidatorService;

        private readonly ILoggingService loggingService;

        public FilePayloadSettingsProvider(IDateTimeService dateTimeService,
                                           IOptions<DownloadSettings> downloadSettings,
                                           IDownloadSettingsValidatorService downloadSettingsValidatorService,
                                           ILoggingService loggingService,
                                           IOptions<DataStoreSettings> dataStoreSettings)
        {
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            this.downloadSettings = downloadSettings?.Value
                                    ?? throw new ArgumentNullException(nameof(downloadSettings));
            this.downloadSettingsValidatorService = downloadSettingsValidatorService
                                                    ?? throw new ArgumentNullException(
                                                        nameof(downloadSettingsValidatorService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.dataStoreSettings =
                dataStoreSettings?.Value ?? throw new ArgumentNullException(nameof(dataStoreSettings));
        }

        public void SetFilePayloadDownloadDetails(FilePayload filePayload)
        {
            DateTime now = DateTimeNow();
            string downloadDirectory = GetDownloadDirectory();
            string uploadDirectory = GetUploadDirectory();

            SetDownloadDetails(filePayload);
            SetDownloadFileDetails(filePayload, now, downloadDirectory);
            SetDecompressedDownloadFileDetails(filePayload, now, downloadDirectory);
            SetFailedDownloadFileDetails(filePayload, now, downloadDirectory);
            SetUploadFileDetails(filePayload, now, uploadDirectory);
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }

        private string GetDownloadDirectory()
        {
            string downloadDirectory = downloadSettings.DownloadDirectory;

            if (!downloadSettingsValidatorService.IsValidDownloadDirectory(downloadDirectory))
            {
                throw NewFileDownloadArgumentException("Download directory is invalid");
            }

            return downloadDirectory;
        }

        private string GetDownloadFileName(DateTime dateTime)
        {
            return $"{dateTime.ToFileTime()}.{Constants.FilePayload.FileName}";
        }

        private string GetDownloadFileNameWithExtension(DateTime dateTime)
        {
            return $"{dateTime.ToFileTime()}.{Constants.FilePayload.FileName}.{Constants.FilePayload.FileExtension}";
        }

        private string GetUploadDirectory()
        {
            return dataStoreSettings.UploadDirectory;
        }

        private Exception NewFileDownloadArgumentException(string message)
        {
            return new FileDownloadArgumentException(message);
        }

        private void SetDecompressedDownloadFileDetails(FilePayload filePayload, DateTime dateTime,
                                                        string downloadDirectory)
        {
            var decompressedFileName = $"{dateTime.ToFileTime()}.{Constants.FilePayload.DecompressedFileName}";

            filePayload.DecompressedDownloadDirectory = downloadDirectory;
            filePayload.DecompressedDownloadFileName = decompressedFileName;
            filePayload.DecompressedDownloadFileExtension = Constants.FilePayload.DecompressedFileExtension;
            filePayload.DecompressedDownloadFilePath = Path.Combine(downloadDirectory,
                $"{decompressedFileName}.{Constants.FilePayload.DecompressedFileExtension}");
        }

        private void SetDownloadDetails(FilePayload filePayload)
        {
            int downloadTimeout = downloadSettings.DownloadTimeout;
            string unsafeDownloadUri = downloadSettings.DownloadUri;
            bool acceptAnySslCert = downloadSettings.AcceptAnySslCert;
            int configuredMinimumWaitTimeInHours = downloadSettings.MinimumWaitTimeInHours;

            if (!TryParseDownloadUri(unsafeDownloadUri, out Uri downloadUri))
            {
                throw NewFileDownloadArgumentException("Download Uri is invalid");
            }

            if (!TryParseTimeout(downloadTimeout, out int timeoutInSeconds))
            {
                timeoutInSeconds = 100;
                loggingService.LogDebug("The download timeout configuration was invalid, using the default value.");
            }

            if (!TryParseMinimumWaitTimeSpan(configuredMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan))
            {
                minimumWaitTimeSpan = MinimumWait.TimeSpan;
                loggingService.LogDebug("The minimum wait time configuration was invalid, using the default value.");
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
            filePayload.DownloadFileExtension = Constants.FilePayload.FileExtension;
            filePayload.DownloadFilePath = Path.Combine(downloadDirectory, GetDownloadFileNameWithExtension(dateTime));
        }

        private void SetFailedDownloadFileDetails(FilePayload filePayload, DateTime dateTime, string downloadDirectory)
        {
            string downloadFileName = GetDownloadFileNameWithExtension(dateTime);

            filePayload.FailedDownloadFilePath =
                Path.Combine(downloadDirectory, "FileDownloadFailed", downloadFileName);
        }

        private void SetUploadFileDetails(FilePayload filePayload, DateTime now, string uploadDirectory)
        {
            filePayload.UploadPath = Path.Combine(uploadDirectory,
                $"{now.ToFileTime()}.{Constants.FilePayload.FileName}.{Constants.FilePayload.FileExtension}");
        }

        private bool TryParseDownloadUri(string unsafeDownloadUri, out Uri downloadUri)
        {
            return downloadSettingsValidatorService.TryParseDownloadUri(unsafeDownloadUri, out downloadUri);
        }

        private bool TryParseMinimumWaitTimeSpan(int minimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan)
        {
            return downloadSettingsValidatorService.TryParseMinimumWaitTimeSpan(minimumWaitTimeInHours,
                out minimumWaitTimeSpan);
        }

        private bool TryParseTimeout(int timeout, out int timeoutInSeconds)
        {
            return downloadSettingsValidatorService.TryParseTimeout(timeout, out timeoutInSeconds);
        }
    }
}