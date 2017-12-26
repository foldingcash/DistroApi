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
                StatsPayload statsPayload = NewStatsPayload();
                LogVerbose($"Stats file download started: {DateTime.Now}");
                DownloadFile(statsPayload);
                LogVerbose($"Stats file download completed: {DateTime.Now}");
                UploadFile(statsPayload);
                FileDownloadResult successResult = NewSuccessFileDownloadResult(statsPayload);
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

        private void DownloadFile(StatsPayload statsPayload)
        {
            downloadService.DownloadFile(statsPayload);
        }

        private string GetDownloadDirectory()
        {
            return fileDownloadSettingsService.GetDownloadDirectory();
        }

        private string GetDownloadFileName()
        {
            string downloadDirectory = GetDownloadDirectory();
            return fileNameService.GetFileDownloadPath(downloadDirectory);
        }

        private string GetDownloadTimeout()
        {
            return fileDownloadSettingsService.GetDownloadTimeout();
        }

        private string GetDownloadUrl()
        {
            return fileDownloadSettingsService.GetDownloadUrl();
        }

        private string GetUncompressedDownloadFilePath()
        {
            string downloadDirectory = GetDownloadDirectory();
            return fileNameService.GetUncompressedFileDownloadPath(downloadDirectory);
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

        private int NewFileDownloadStarted()
        {
            return fileDownloadDataStoreService.NewFileDownloadStarted();
        }

        private StatsPayload NewStatsPayload()
        {
            int downloadId = NewFileDownloadStarted();

            string downloadUrl = GetDownloadUrl();
            string downloadTimeout = GetDownloadTimeout();
            string downloadFileName = GetDownloadFileName();
            string uncompressedDownloadFilePath = GetUncompressedDownloadFilePath();

            int timeoutInSeconds;
            TryParseTimeout(downloadTimeout, out timeoutInSeconds);

            return new StatsPayload(
                downloadId,
                downloadUrl,
                timeoutInSeconds,
                downloadFileName,
                uncompressedDownloadFilePath);
        }

        private FileDownloadResult NewSuccessFileDownloadResult(StatsPayload statsPayload)
        {
            return new FileDownloadResult(statsPayload);
        }

        private bool TryParseTimeout(string unsafeTimeout, out int timeoutInSeconds)
        {
            return fileDownloadTimeoutValidatorService.TryParseTimeout(unsafeTimeout, out timeoutInSeconds);
        }

        private void UpdateToLatest()
        {
            fileDownloadDataStoreService.UpdateToLatest();
        }

        private void UploadFile(StatsPayload statsPayload)
        {
            fileCompressionService.DecompressFile(statsPayload);
            fileReaderService.ReadFile(statsPayload);
        }
    }
}