namespace StatsDownload.Core
{
    using System;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileDownloadLoggingService fileDownloadLoggingService;

        private readonly IFileDownloadSettingsService fileDownloadSettingsService;

        private readonly IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService;

        private readonly IFileNameService fileNameService;

        private readonly IFileReaderService fileReaderService;

        public FileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService,
            IFileDownloadSettingsService fileDownloadSettingsService,
            IDownloadService downloadService,
            IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService,
            IFileNameService fileNameService,
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService)
        {
            if (IsNull(fileDownloadDataStoreService))
            {
                throw NewArgumentNullException(nameof(fileDownloadDataStoreService));
            }

            if (IsNull(fileDownloadLoggingService))
            {
                throw NewArgumentNullException(nameof(fileDownloadLoggingService));
            }

            if (IsNull(fileDownloadSettingsService))
            {
                throw NewArgumentNullException(nameof(fileDownloadSettingsService));
            }

            if (IsNull(downloadService))
            {
                throw NewArgumentNullException(nameof(downloadService));
            }

            if (IsNull(fileDownloadTimeoutValidatorService))
            {
                throw NewArgumentNullException(nameof(fileDownloadTimeoutValidatorService));
            }

            if (IsNull(fileNameService))
            {
                throw NewArgumentNullException(nameof(fileNameService));
            }

            if (IsNull(fileCompressionService))
            {
                throw NewArgumentNullException(nameof(fileCompressionService));
            }

            if (IsNull(fileReaderService))
            {
                throw NewArgumentNullException(nameof(fileReaderService));
            }

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.fileDownloadLoggingService = fileDownloadLoggingService;
            this.fileDownloadSettingsService = fileDownloadSettingsService;
            this.downloadService = downloadService;
            this.fileDownloadTimeoutValidatorService = fileDownloadTimeoutValidatorService;
            this.fileNameService = fileNameService;
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
        }

        public FileDownloadResult DownloadStatsFile()
        {
            try
            {
                LogMethodInvoked(nameof(DownloadStatsFile));

                if (DataStoreUnavailable())
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(FailedReason.DataStoreUnavailable);
                    LogResult(failedResult);
                    return failedResult;
                }

                UpdateToLatest();
                FilePayload filePayload = NewStatsPayload();
                LogVerbose($"Stats file download started: {DateTime.Now}");
                DownloadFile(filePayload);
                LogVerbose($"Stats file download completed: {DateTime.Now}");
                UploadFile(filePayload);
                FileDownloadResult successResult = NewSuccessFileDownloadResult(filePayload);
                LogResult(successResult);

                return successResult;
            }
            catch (Exception exception)
            {
                FileDownloadResult result = NewFailedFileDownloadResult(FailedReason.UnexpectedException);
                LogResult(result);
                LogException(exception);
                return result;
            }
        }

        private bool DataStoreUnavailable()
        {
            return !fileDownloadDataStoreService.IsAvailable();
        }

        private void DownloadFile(FilePayload filePayload)
        {
            downloadService.DownloadFile(filePayload);
        }

        private string GetDownloadDirectory()
        {
            return fileDownloadSettingsService.GetDownloadDirectory();
        }

        private string GetDownloadTimeout()
        {
            return fileDownloadSettingsService.GetDownloadTimeout();
        }

        private string GetDownloadUrl()
        {
            return fileDownloadSettingsService.GetDownloadUrl();
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private void LogException(Exception exception)
        {
            fileDownloadLoggingService.LogException(exception);
        }

        private void LogMethodInvoked(string method)
        {
            LogVerbose($"{method} Invoked");
        }

        private void LogResult(FileDownloadResult result)
        {
            fileDownloadLoggingService.LogResult(result);
        }

        private void LogVerbose(string message)
        {
            fileDownloadLoggingService.LogVerbose(message);
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private FileDownloadResult NewFailedFileDownloadResult(FailedReason failedReason)
        {
            return new FileDownloadResult(failedReason);
        }

        private void NewFileDownloadStarted(FilePayload filePayload)
        {
            fileDownloadDataStoreService.NewFileDownloadStarted(filePayload);
        }

        private FilePayload NewStatsPayload()
        {
            var filePayload = new FilePayload();

            string downloadDirectory = GetDownloadDirectory();

            SetDownloadFileDetails(downloadDirectory, filePayload);

            NewFileDownloadStarted(filePayload);

            string downloadUrl = GetDownloadUrl();
            string downloadTimeout = GetDownloadTimeout();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            filePayload.DownloadUrl = downloadUrl;
            filePayload.TimeoutSeconds = timeoutInSeconds;

            return filePayload;
        }

        private FileDownloadResult NewSuccessFileDownloadResult(FilePayload filePayload)
        {
            return new FileDownloadResult(filePayload);
        }

        private void SetDownloadFileDetails(string downloadDirectory, FilePayload filePayload)
        {
            fileNameService.SetDownloadFileDetails(downloadDirectory, filePayload);
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return fileDownloadTimeoutValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }

        private void UpdateToLatest()
        {
            fileDownloadDataStoreService.UpdateToLatest();
        }

        private void UploadFile(FilePayload filePayload)
        {
            fileCompressionService.DecompressFile(filePayload);
            fileReaderService.ReadFile(filePayload);
            fileDownloadDataStoreService.FileDownloadFinished(filePayload);
        }
    }
}