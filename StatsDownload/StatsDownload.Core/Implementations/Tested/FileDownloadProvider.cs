namespace StatsDownload.Core
{
    using System;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IDateTimeService dateTimeService;

        private readonly IDownloadService downloadService;

        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService;

        private readonly IFilePayloadSettingsService filePayloadSettingsService;

        private readonly IFileReaderService fileReaderService;

        private readonly ILoggingService loggingService;

        private readonly IResourceCleanupService resourceCleanupService;

        public FileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            ILoggingService loggingService,
            IDownloadService downloadService,
            IFilePayloadSettingsService filePayloadSettingsService,
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService,
            IResourceCleanupService resourceCleanupService,
            IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
            IDateTimeService dateTimeService)
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

            if (IsNull(fileCompressionService))
            {
                throw NewArgumentNullException(nameof(fileCompressionService));
            }

            if (IsNull(fileReaderService))
            {
                throw NewArgumentNullException(nameof(fileReaderService));
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

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.loggingService = loggingService;
            this.downloadService = downloadService;
            this.filePayloadSettingsService = filePayloadSettingsService;
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
            this.resourceCleanupService = resourceCleanupService;
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService;
            this.dateTimeService = dateTimeService;
        }

        public FileDownloadResult DownloadStatsFile()
        {
            FilePayload filePayload = NewStatsPayload();

            try
            {
                LogMethodInvoked(nameof(DownloadStatsFile));

                if (DataStoreUnavailable())
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(
                        FailedReason.DataStoreUnavailable,
                        filePayload);
                    LogResult(failedResult);
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
                    return failedResult;
                }

                SetDownloadFileDetails(filePayload);
                DownloadFile(filePayload);
                LogVerbose($"Stats file download completed: {DateTimeNow()}");
                UploadFile(filePayload);
                Cleanup(filePayload);
                FileDownloadResult successResult = NewSuccessFileDownloadResult(filePayload);
                LogResult(successResult);

                return successResult;
            }
            catch (Exception exception)
            {
                FileDownloadResult result = NewFailedFileDownloadResult(FailedReason.UnexpectedException, filePayload);
                LogResult(result);
                LogException(exception);
                Cleanup(filePayload);
                return result;
            }
        }

        private void Cleanup(FilePayload filePayload)
        {
            resourceCleanupService.Cleanup(filePayload);
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
            fileCompressionService.DecompressFile(filePayload);
            fileReaderService.ReadFile(filePayload);
            fileDownloadDataStoreService.FileDownloadFinished(filePayload);
        }
    }
}