namespace StatsDownload.Core
{
    using System;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileDownloadLoggingService fileDownloadLoggingService;

        public FileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            IFileDownloadLoggingService fileDownloadLoggingService)
        {
            if (IsNull(fileDownloadDataStoreService))
            {
                throw NewArgumentNullException(nameof(fileDownloadDataStoreService));
            }

            if (IsNull(fileDownloadLoggingService))
            {
                throw NewArgumentNullException(nameof(fileDownloadLoggingService));
            }

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.fileDownloadLoggingService = fileDownloadLoggingService;
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

                FileDownloadResult successResult = NewSuccessFileDownloadResult();
                LogResult(successResult);
                return successResult;
            }
            catch (Exception exception)
            {
                FileDownloadResult result = NewFailedFileDownloadResult(FailedReason.UnexpectedException);
                LogException(exception);
                return result;
            }
        }

        private bool DataStoreUnavailable()
        {
            return !fileDownloadDataStoreService.IsAvailable();
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

        private FileDownloadResult NewSuccessFileDownloadResult()
        {
            return new FileDownloadResult();
        }

        private void UpdateToLatest()
        {
            fileDownloadDataStoreService.UpdateToLatest();
        }
    }
}