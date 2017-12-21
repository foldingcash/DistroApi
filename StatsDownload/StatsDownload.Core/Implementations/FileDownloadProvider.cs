namespace StatsDownload.Core
{
    using System;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileDownloadLoggingService fileDownloadLoggingService;

        private readonly IFileDownloadSettingsService fileDownloadSettingsService;

        public FileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService,
            IFileDownloadSettingsService fileDownloadSettingsService)
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

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.fileDownloadLoggingService = fileDownloadLoggingService;
            this.fileDownloadSettingsService = fileDownloadSettingsService;
        }

        public FileDownloadResult DownloadFile()
        {
            try
            {
                LogMethodInvoked(nameof(DownloadFile));

                if (DataStoreUnavailable())
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(FailedReason.DataStoreUnavailable);
                    LogResult(failedResult);
                    return failedResult;
                }

                UpdateToLatest();

                int downloadId = NewFileDownloadStarted();

                string downloadUrl = GetDownloadUrl();
                string downloadTimeout = GetDownloadTimeout();
                string downloadDirectory = GetDownloadDirectory();

                FileDownloadResult successResult = NewSuccessFileDownloadResult(
                    downloadId,
                    downloadUrl,
                    downloadTimeout,
                    downloadDirectory);
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

        private int NewFileDownloadStarted()
        {
            return fileDownloadDataStoreService.NewFileDownloadStarted();
        }

        private FileDownloadResult NewSuccessFileDownloadResult(
            int downloadId,
            string downloadUrl,
            string downloadTimeout,
            string downloadDirectory)
        {
            return new FileDownloadResult(downloadId, downloadUrl, downloadTimeout, downloadDirectory);
        }

        private void UpdateToLatest()
        {
            fileDownloadDataStoreService.UpdateToLatest();
        }
    }
}