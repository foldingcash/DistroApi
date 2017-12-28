namespace StatsDownload.Core
{
    using System;

    public class FileDownloadProvider : IFileDownloadService
    {
        private readonly IDownloadService downloadService;

        private readonly IFileCompressionService fileCompressionService;

        private readonly IFileDownloadDataStoreService fileDownloadDataStoreService;

        private readonly IFilePayloadSettingsService filePayloadSettingsService;

        private readonly IFileReaderService fileReaderService;

        private readonly ILoggingService loggingService;

        public FileDownloadProvider(
            IFileDownloadDataStoreService fileDownloadDataStoreService,
            ILoggingService loggingService,
            IDownloadService downloadService,
            IFilePayloadSettingsService filePayloadSettingsService,
            IFileCompressionService fileCompressionService,
            IFileReaderService fileReaderService)
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

            this.fileDownloadDataStoreService = fileDownloadDataStoreService;
            this.loggingService = loggingService;
            this.downloadService = downloadService;
            this.filePayloadSettingsService = filePayloadSettingsService;
            this.fileCompressionService = fileCompressionService;
            this.fileReaderService = fileReaderService;
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
                LogVerbose($"Stats file download started: {DateTime.Now}");
                NewFileDownloadStarted(filePayload);
                SetDownloadFileDetails(filePayload);
                DownloadFile(filePayload);
                LogVerbose($"Stats file download completed: {DateTime.Now}");
                UploadFile(filePayload);
                FileDownloadResult successResult = NewSuccessFileDownloadResult(filePayload);
                LogResult(successResult);

                return successResult;
            }
            catch (Exception exception)
            {
                FileDownloadResult result = NewFailedFileDownloadResult(FailedReason.UnexpectedException, filePayload);
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