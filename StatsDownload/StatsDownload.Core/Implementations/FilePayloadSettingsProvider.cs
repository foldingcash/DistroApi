namespace StatsDownload.Core.Implementations
{
    using System;
    using System.IO;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Core.Interfaces.Logging;

    public class FilePayloadSettingsProvider : IFilePayloadSettingsService
    {
        private readonly IDataStoreSettings dataStoreSettings;

        private readonly IDateTimeService dateTimeService;

        private readonly IDownloadSettingsService downloadSettingsService;

        private readonly IDownloadSettingsValidatorService downloadSettingsValidatorService;

        private readonly ILoggingService loggingService;

        public FilePayloadSettingsProvider(IDateTimeService dateTimeService,
                                           IDownloadSettingsService downloadSettingsService,
                                           IDownloadSettingsValidatorService downloadSettingsValidatorService,
                                           ILoggingService loggingService, IDataStoreSettings dataStoreSettings)
        {
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            this.downloadSettingsService = downloadSettingsService
                                           ?? throw new ArgumentNullException(nameof(downloadSettingsService));
            this.downloadSettingsValidatorService = downloadSettingsValidatorService
                                                    ?? throw new ArgumentNullException(
                                                        nameof(downloadSettingsValidatorService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.dataStoreSettings = dataStoreSettings ?? throw new ArgumentNullException(nameof(dataStoreSettings));
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
            return $"{dateTime.ToFileTime()}.{Constants.FilePayload.FileName}";
        }

        private string GetDownloadFileNameWithExtension(DateTime dateTime)
        {
            return $"{dateTime.ToFileTime()}.{Constants.FilePayload.FileName}.{Constants.FilePayload.FileExtension}";
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
            string decompressedFileName = $"{dateTime.ToFileTime()}.{Constants.FilePayload.DecompressedFileName}";

            filePayload.DecompressedDownloadDirectory = downloadDirectory;
            filePayload.DecompressedDownloadFileName = decompressedFileName;
            filePayload.DecompressedDownloadFileExtension = Constants.FilePayload.DecompressedFileExtension;
            filePayload.DecompressedDownloadFilePath = Path.Combine(downloadDirectory,
                $"{decompressedFileName}.{Constants.FilePayload.DecompressedFileExtension}");
        }

        private void SetDownloadDetails(FilePayload filePayload)
        {
            string downloadTimeout = GetDownloadTimeout();
            string unsafeDownloadUri = GetDownloadUri();
            string unsafeAcceptAnySslCert = GetAcceptAnySslCert();
            string unsafeMinimumWaitTimeInHours = GetMinimumWaitTimeInHours();

            if (!TryParseDownloadUri(unsafeDownloadUri, out Uri downloadUri))
            {
                throw NewFileDownloadArgumentException("Download Uri is invalid");
            }

            if (!TryParseTimeout(downloadTimeout, out int timeoutInSeconds))
            {
                timeoutInSeconds = 100;
                loggingService.LogVerbose("The download timeout configuration was invalid, using the default value.");
            }

            if (!TryParseAcceptAnySslCert(unsafeAcceptAnySslCert, out bool acceptAnySslCert))
            {
                acceptAnySslCert = false;
                loggingService.LogVerbose(
                    "The accept any SSL cert configuration was invalid, using the default value.");
            }

            if (!TryParseMinimumWaitTimeSpan(unsafeMinimumWaitTimeInHours, out TimeSpan minimumWaitTimeSpan))
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