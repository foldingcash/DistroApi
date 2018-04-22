namespace StatsDownload.Core
{
    using StatsDownload.Core.Interfaces;
    using System;
    using System.Net;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IDownloadService downloadService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileDownloadEmailService fileDownloadEmailService;

        private readonly IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService;

        private readonly IFilePayloadSettingsService filePayloadSettingsService;

        private readonly IFilePayloadUploadService filePayloadUploadService;

        private readonly IFileDownloadLoggingService loggingService;

        private readonly IResourceCleanupService resourceCleanupService;

        public FileDownloadProvider(IFileDownloadDataStoreService fileDownloadDataStoreService,
                                    IFileDownloadLoggingService loggingService, IDownloadService downloadService,
                                    IFilePayloadSettingsService filePayloadSettingsService,
                                    IResourceCleanupService resourceCleanupService,
                                    IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
                                    IDateTimeService dateTimeService, IFilePayloadUploadService filePayloadUploadService,
                                    IFileDownloadEmailService fileDownloadEmailService)
        {
            if (IsNull(fileDownloadDataStoreService))
            {
                throw NewArgumentNullException(nameof(fileDownloadDataStoreService));
            }

            if (IsNull(loggingService))
            {
                throw NewArgumentNullException(nameof(loggingService));
            }

            if (IsNull(downloadService))
            {
                throw NewArgumentNullException(nameof(downloadService));
            }

            if (IsNull(filePayloadSettingsService))
            {
                throw NewArgumentNullException(nameof(filePayloadSettingsService));
            }

            if (IsNull(resourceCleanupService))
            {
                throw NewArgumentNullException(nameof(resourceCleanupService));
            }

            if (IsNull(fileDownloadMinimumWaitTimeService))
            {
                throw NewArgumentNullException(nameof(fileDownloadMinimumWaitTimeService));
            }

            if (IsNull(dateTimeService))
            {
                throw NewArgumentNullException(nameof(dateTimeService));
            }

            if (IsNull(filePayloadUploadService))
            {
                throw NewArgumentNullException(nameof(filePayloadUploadService));
            }

            if (IsNull(fileDownloadEmailService))
            {
                throw NewArgumentNullException(nameof(fileDownloadEmailService));
            }

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.loggingService = loggingService;
            this.downloadService = downloadService;
            this.filePayloadSettingsService = filePayloadSettingsService;
            this.resourceCleanupService = resourceCleanupService;
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService;
            this.dateTimeService = dateTimeService;
            this.filePayloadUploadService = filePayloadUploadService;
            this.fileDownloadEmailService = fileDownloadEmailService;
        }

        public FileDownloadResult DownloadStatsFile()
        {
            FilePayload filePayload = NewStatsPayload();

            try
            {
                LogMethodInvoked(nameof(DownloadStatsFile));

                if (DataStoreUnavailable())
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(FailedReason.DataStoreUnavailable,
                        filePayload);
                    LogResult(failedResult);
                    SendEmail(failedResult);
                    return failedResult;
                }

                UpdateToLatest();
                LogVerbose($"Stats file download started: {DateTimeNow()}");
                NewFileDownloadStarted(filePayload);

                FailedReason failedReason;
                if (IsFileDownloadNotReadyToRun(filePayload, out failedReason))
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(failedReason, filePayload);
                    LogResult(failedResult);
                    FileDownloadError(failedResult);
                    SendEmail(failedResult);
                    return failedResult;
                }

                SetDownloadFileDetails(filePayload);
                DownloadFile(filePayload);
                LogVerbose($"Stats file download completed: {DateTimeNow()}");
                UploadFile(filePayload);
                FileDownloadResult successResult = NewSuccessFileDownloadResult(filePayload);
                Cleanup(successResult);
                LogResult(successResult);
                return successResult;
            }
            catch (Exception exception)
            {
                FileDownloadResult exceptionResult = NewFailedFileDownloadResult(exception, filePayload);
                LogResult(exceptionResult);
                LogException(exception);
                FileDownloadError(exceptionResult);
                Cleanup(exceptionResult);
                SendEmail(exceptionResult);
                return exceptionResult;
            }
        }

        private void Cleanup(FileDownloadResult fileDownloadResult)
        {
            resourceCleanupService.Cleanup(fileDownloadResult);
        }

        private bool DataStoreUnavailable()
        {
            return !fileDownloadDataStoreService.IsAvailable();
        }

        private DateTime DateTimeNow()
        {
            return dateTimeService.DateTimeNow();
        }

        private void DownloadFile(FilePayload filePayload)
        {
            downloadService.DownloadFile(filePayload);
        }

        private void FileDownloadError(FileDownloadResult fileDownloadResult)
        {
            fileDownloadDataStoreService.FileDownloadError(fileDownloadResult);
        }

        private bool IsFileDownloadNotReadyToRun(FilePayload filePayload, out FailedReason failedReason)
        {
            return !IsFileDownloadReadyToRun(filePayload, out failedReason);
        }

        private bool IsFileDownloadReadyToRun(FilePayload filePayload, out FailedReason failedReason)
        {
            if (IsMinimumWaitTimeNotMet(filePayload))
            {
                failedReason = FailedReason.MinimumWaitTimeNotMet;
                return false;
            }

            failedReason = FailedReason.None;
            return true;
        }

        private bool IsMinimumWaitTimeNotMet(FilePayload filePayload)
        {
            return !fileDownloadMinimumWaitTimeService.IsMinimumWaitTimeMet(filePayload);
        }

        private bool IsNull(object value)
        {
            return value == null;
        }

        private void LogException(Exception exception)
        {
            loggingService.LogException(exception);
        }

        private void LogMethodInvoked(string method)
        {
            LogVerbose($"{method} Invoked");
        }

        private void LogResult(FileDownloadResult result)
        {
            loggingService.LogResult(result);
        }

        private void LogVerbose(string message)
        {
            loggingService.LogVerbose(message);
        }

        private Exception NewArgumentNullException(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        private FileDownloadResult NewFailedFileDownloadResult(Exception exception, FilePayload filePayload)
        {
            var webException = exception as WebException;
            if (webException?.Status == WebExceptionStatus.Timeout)
            {
                return NewFailedFileDownloadResult(FailedReason.FileDownloadTimeout, filePayload);
            }
            if (exception is FileDownloadFailedDecompressionException)
            {
                return NewFailedFileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);
            }
            if (exception is FileDownloadArgumentException)
            {
                return NewFailedFileDownloadResult(FailedReason.RequiredSettingsInvalid, filePayload);
            }
            return NewFailedFileDownloadResult(FailedReason.UnexpectedException, filePayload);
        }

        private FileDownloadResult NewFailedFileDownloadResult(FailedReason failedReason, FilePayload filePayload)
        {
            return new FileDownloadResult(failedReason, filePayload);
        }

        private void NewFileDownloadStarted(FilePayload filePayload)
        {
            fileDownloadDataStoreService.NewFileDownloadStarted(filePayload);
        }

        private FilePayload NewStatsPayload()
        {
            return new FilePayload();
        }

        private FileDownloadResult NewSuccessFileDownloadResult(FilePayload filePayload)
        {
            return new FileDownloadResult(filePayload);
        }

        private void SendEmail(FileDownloadResult fileDownloadResult)
        {
            fileDownloadEmailService.SendEmail(fileDownloadResult);
        }

        private void SetDownloadFileDetails(FilePayload filePayload)
        {
            filePayloadSettingsService.SetFilePayloadDownloadDetails(filePayload);
        }

        private void UpdateToLatest()
        {
            fileDownloadDataStoreService.UpdateToLatest();
        }

        private void UploadFile(FilePayload filePayload)
        {
            filePayloadUploadService.UploadFile(filePayload);
        }
    }
}