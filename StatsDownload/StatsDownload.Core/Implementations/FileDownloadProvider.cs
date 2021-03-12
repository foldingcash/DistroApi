namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Logging;

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

        private readonly ILogger<FileDownloadProvider> logger;

        private readonly IFileDownloadLoggingService loggingService;

        private readonly IResourceCleanupService resourceCleanupService;

        public FileDownloadProvider(ILogger<FileDownloadProvider> logger,
                                    IFileDownloadDatabaseService fileDownloadDatabaseService,
                                    IFileDownloadLoggingService loggingService, IDownloadService downloadService,
                                    IFilePayloadSettingsService filePayloadSettingsService,
                                    IResourceCleanupService resourceCleanupService,
                                    IFileDownloadMinimumWaitTimeService fileDownloadMinimumWaitTimeService,
                                    IDateTimeService dateTimeService,
                                    IFilePayloadUploadService filePayloadUploadService,
                                    IFileDownloadEmailService fileDownloadEmailService,
                                    IDataStoreServiceFactory dataStoreServiceFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.fileDownloadDatabaseService = fileDownloadDatabaseService
                                               ?? throw new ArgumentNullException(nameof(fileDownloadDatabaseService));
            this.loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            this.downloadService = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
            this.filePayloadSettingsService = filePayloadSettingsService
                                              ?? throw new ArgumentNullException(nameof(filePayloadSettingsService));
            this.resourceCleanupService =
                resourceCleanupService ?? throw new ArgumentNullException(nameof(resourceCleanupService));
            this.fileDownloadMinimumWaitTimeService = fileDownloadMinimumWaitTimeService
                                                      ?? throw new ArgumentNullException(
                                                          nameof(fileDownloadMinimumWaitTimeService));
            this.dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            this.filePayloadUploadService = filePayloadUploadService
                                            ?? throw new ArgumentNullException(nameof(filePayloadUploadService));
            this.fileDownloadEmailService = fileDownloadEmailService
                                            ?? throw new ArgumentNullException(nameof(fileDownloadEmailService));

            dataStoreService = dataStoreServiceFactory?.Create()
                               ?? throw new ArgumentNullException(nameof(dataStoreServiceFactory));
        }

        public async Task<FileDownloadResult> DownloadStatsFile()
        {
            FilePayload filePayload = NewStatsPayload();

            try
            {
                logger.LogMethodInvoked();

                (bool isAvailable, FailedReason failedReason) result = await IsDataStoreAvailable();
                bool dataStoreUnavailable = !result.isAvailable;
                FailedReason failedReason = result.failedReason;

                if (dataStoreUnavailable || DatabaseUnavailable(out failedReason))
                {
                    FileDownloadResult failedResult = NewFailedFileDownloadResult(failedReason, filePayload);
                    LogResult(failedResult);
                    SendEmail(failedResult);
                    return failedResult;
                }

                fileDownloadDatabaseService.UpdateToLatest();
                logger.LogDebug($"Stats file download started: {DateTimeNow()}");
                FileDownloadStarted(filePayload);

                if (IsFileDownloadNotReadyToRun(filePayload, out failedReason))
                {
                    return HandleDownloadNotReadyToRun(failedReason, filePayload);
                }

                filePayloadSettingsService.SetFilePayloadDownloadDetails(filePayload);
                DownloadFile(filePayload);
                return await HandleSuccessAndUpload(filePayload);
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

        private async Task<FileDownloadResult> HandleSuccessAndUpload(FilePayload filePayload)
        {
            logger.LogDebug($"Stats file download completed: {DateTimeNow()}");
            await filePayloadUploadService.UploadFile(filePayload);
            var successResult = new FileDownloadResult(filePayload);
            Cleanup(successResult);
            LogResult(successResult);
            return successResult;
        }

        private async Task<(bool isAvailable, FailedReason failedReason)> IsDataStoreAvailable()
        {
            bool isAvailable = await dataStoreService.IsAvailable();
            FailedReason failedReason = isAvailable ? FailedReason.None : FailedReason.DataStoreUnavailable;
            return (isAvailable, failedReason);
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
            logger.LogError(exception, "There was an unhandled exception");
        }

        private void LogResult(FileDownloadResult result)
        {
            loggingService.LogResult(result);
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

        private void SendEmail(FileDownloadResult fileDownloadResult)
        {
            fileDownloadEmailService.SendEmail(fileDownloadResult);
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
    }
}