namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Net;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IDataStoreService dataStoreService;

        private readonly IDateTimeService dateTimeService;

        private readonly IDownloadService downloadService;

        private readonly IFileDownloadDatabaseService fileDownloadDatabaseService;

        private readonly IFileDownloadEmailService fileDownloadEmailService;

        private readonly IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService;

        private readonly IFilePayloadSettingsService filePayloadSettingsService;

        private readonly IFilePayloadUploadService filePayloadUploadService;

        private readonly IFileDownloadLoggingService loggingService;

        private readonly IResourceCleanupService resourceCleanupService;

        public FileDownloadProvider(IFileDownloadDatabaseService fileDownloadDatabaseService,
                                    IFileDownloadLoggingService loggingService, IDownloadService downloadService,
                                    IFilePayloadSettingsService filePayloadSettingsService,
                                    IResourceCleanupService resourceCleanupService,
                                    IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
                                    IDateTimeService dateTimeService,
                                    IFilePayloadUploadService filePayloadUploadService,
                                    IFileDownloadEmailService fileDownloadEmailService,
                                    IDataStoreServiceFactory dataStoreServiceFactory)
        {
            this.fileDownloadDatabaseService = fileDownloadDatabaseService;
            this.loggingService = loggingService;
            this.downloadService = downloadService;
            this.filePayloadSettingsService = filePayloadSettingsService;
            this.resourceCleanupService = resourceCleanupService;
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService;
            this.dateTimeService = dateTimeService;
            this.filePayloadUploadService = filePayloadUploadService;
            this.fileDownloadEmailService = fileDownloadEmailService;

            dataStoreService = dataStoreServiceFactory?.Create();

            ValidateCtorArgs();
        }

        public FileDownloadResult DownloadStatsFile()
        {
            FilePayload filePayload = NewStatsPayload();

            try
            {
                LogMethodInvoked(nameof(DownloadStatsFile));

                if (DatabaseUnavailable(out FailedReason failedReason) || DataStoreUnavailable(out failedReason))
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(failedReason, filePayload);
                    LogResult(failedResult);
                    SendEmail(failedResult);
                    return failedResult;
                }

                UpdateToLatest();
                LogVerbose($"Stats file download started: {DateTimeNow()}");
                FileDownloadStarted(filePayload);

                if (IsFileDownloadNotReadyToRun(filePayload, out failedReason))
                {
                    return HandleDownloadNotReadyToRun(failedReason, filePayload);
                }

                SetDownloadFileDetails(filePayload);
                DownloadFile(filePayload);
                return HandleSuccessAndUpload(filePayload);
            }
            catch (Exception exception)
            {
                return HandleException(exception, filePayload);
            }
        }

        private void Cleanup(FileDownloadResult fileDownloadResult)
        {
            resourceCleanupService.Cleanup(fileDownloadResult);
        }

        private bool DatabaseUnavailable(out FailedReason failedReason)
        {
            (bool isAvailable, FailedReason reason) = fileDownloadDatabaseService.IsAvailable();
            failedReason = reason;
            return !isAvailable;
        }

        private bool DataStoreUnavailable(out FailedReason failedReason)
        {
            (bool isAvailable, FailedReason reason) = dataStoreService.IsAvailable();
            failedReason = reason;
            return !isAvailable;
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
            fileDownloadDatabaseService.FileDownloadError(fileDownloadResult);
        }

        private void FileDownloadStarted(FilePayload filePayload)
        {
            fileDownloadDatabaseService.FileDownloadStarted(filePayload);
        }

        private FileDownloadResult HandleDownloadNotReadyToRun(FailedReason failedReason, FilePayload filePayload)
        {
            FileDownloadResult failedResult = NewFailedFileDownloadResult(failedReason, filePayload);
            LogResult(failedResult);
            FileDownloadError(failedResult);
            SendEmail(failedResult);
            return failedResult;
        }

        private FileDownloadResult HandleException(Exception exception, FilePayload filePayload)
        {
            FileDownloadResult exceptionResult = NewFailedFileDownloadResult(exception, filePayload);
            LogResult(exceptionResult);
            LogException(exception);
            UpdateToError(exceptionResult);
            Cleanup(exceptionResult);
            SendEmail(exceptionResult);
            return exceptionResult;
        }

        private FileDownloadResult HandleSuccessAndUpload(FilePayload filePayload)
        {
            LogVerbose($"Stats file download completed: {DateTimeNow()}");
            UploadFile(filePayload);
            FileDownloadResult successResult = NewSuccessFileDownloadResult(filePayload);
            Cleanup(successResult);
            LogResult(successResult);
            return successResult;
        }

        private bool IsFileDownloadError(FileDownloadResult exceptionResult)
        {
            return exceptionResult.FailedReason != FailedReason.FileDownloadFailedDecompression
                   && exceptionResult.FailedReason != FailedReason.InvalidStatsFileUpload
                   && exceptionResult.FailedReason != FailedReason.UnexpectedValidationException;
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

        private FileDownloadResult NewFailedFileDownloadResult(Exception exception, FilePayload filePayload)
        {
            var webException = exception as WebException;
            if (webException?.Status == WebExceptionStatus.Timeout)
            {
                return NewFailedFileDownloadResult(FailedReason.FileDownloadTimeout, filePayload);
            }

            if (webException?.Status == WebExceptionStatus.ConnectFailure
                || webException?.Status == WebExceptionStatus.ProtocolError)
            {
                return NewFailedFileDownloadResult(FailedReason.FileDownloadNotFound, filePayload);
            }

            if (exception is FileDownloadFailedDecompressionException)
            {
                return NewFailedFileDownloadResult(FailedReason.FileDownloadFailedDecompression, filePayload);
            }

            if (exception is FileDownloadArgumentException)
            {
                return NewFailedFileDownloadResult(FailedReason.RequiredSettingsInvalid, filePayload);
            }

            if (exception is InvalidStatsFileException)
            {
                return NewFailedFileDownloadResult(FailedReason.InvalidStatsFileUpload, filePayload);
            }

            if (exception is UnexpectedValidationException)
            {
                return NewFailedFileDownloadResult(FailedReason.UnexpectedValidationException, filePayload);
            }

            return NewFailedFileDownloadResult(FailedReason.UnexpectedException, filePayload);
        }

        private FileDownloadResult NewFailedFileDownloadResult(FailedReason failedReason, FilePayload filePayload)
        {
            return new FileDownloadResult(failedReason, filePayload);
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

        private void UpdateToError(FileDownloadResult exceptionResult)
        {
            if (IsFileDownloadError(exceptionResult))
            {
                fileDownloadDatabaseService.FileDownloadError(exceptionResult);
            }
            else
            {
                fileDownloadDatabaseService.FileValidationError(exceptionResult);
            }
        }

        private void UpdateToLatest()
        {
            fileDownloadDatabaseService.UpdateToLatest();
        }

        private void UploadFile(FilePayload filePayload)
        {
            filePayloadUploadService.UploadFile(filePayload);
        }

        private void ValidateCtorArgs()
        {
            if (fileDownloadDatabaseService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            if (downloadService == null)
            {
                throw new ArgumentNullException(nameof(downloadService));
            }

            if (filePayloadSettingsService == null)
            {
                throw new ArgumentNullException(nameof(filePayloadSettingsService));
            }

            if (resourceCleanupService == null)
            {
                throw new ArgumentNullException(nameof(resourceCleanupService));
            }

            if (fileDownloadMinimumWaitTimeService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadMinimumWaitTimeService));
            }

            if (dateTimeService == null)
            {
                throw new ArgumentNullException(nameof(dateTimeService));
            }

            if (filePayloadUploadService == null)
            {
                throw new ArgumentNullException(nameof(filePayloadUploadService));
            }

            if (fileDownloadEmailService == null)
            {
                throw new ArgumentNullException(nameof(fileDownloadEmailService));
            }

            if (dataStoreService == null)
            {
                throw new ArgumentNullException(nameof(dataStoreService));
            }
        }
    }
}