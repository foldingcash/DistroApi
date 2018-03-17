namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;

    public class StatsUploadProvider : IStatsUploadService
    {
        private readonly IStatsUploadLoggingService loggingService;

        private readonly IStatsFileParserService statsFileParserService;

        private readonly IStatsUploadDataStoreService statsUploadDataStoreService;

        private readonly IStatsUploadEmailService statsUploadEmailService;

        public StatsUploadProvider(IStatsUploadDataStoreService statsUploadDataStoreService,
                                   IStatsUploadLoggingService loggingService,
                                   IStatsFileParserService statsFileParserService,
                                   IStatsUploadEmailService statsUploadEmailService)
        {
            this.statsUploadDataStoreService = statsUploadDataStoreService;
            this.loggingService = loggingService;
            this.statsFileParserService = statsFileParserService;
            this.statsUploadEmailService = statsUploadEmailService;
        }

        public StatsUploadResults UploadStatsFiles()
        {
            try
            {
                loggingService.LogVerbose($"{nameof(UploadStatsFiles)} Invoked");
                if (DataStoreUnavailable())
                {
                    return new StatsUploadResults(FailedReason.DataStoreUnavailable);
                }

                List<int> uploadFiles = statsUploadDataStoreService.GetDownloadsReadyForUpload();
                var statsUploadResults = new List<StatsUploadResult>();

                foreach (int uploadFile in uploadFiles)
                {
                    try
                    {
                        loggingService.LogVerbose($"Starting stats file upload. DownloadId: {uploadFile}");
                        statsUploadDataStoreService.StartStatsUpload(uploadFile);
                        string fileData = statsUploadDataStoreService.GetFileData(uploadFile);
                        List<UserData> usersData = statsFileParserService.Parse(fileData);

                        foreach (UserData userData in usersData)
                        {
                            statsUploadDataStoreService.AddUserData(userData);
                        }
                        statsUploadDataStoreService.StatsUploadFinished(uploadFile);
                        loggingService.LogVerbose($"Finished stats file upload. DownloadId: {uploadFile}");
                    }
                    catch (Exception exception)
                    {
                        StatsUploadResult failedStatsUploadResult = NewFailedStatsUploadResult(exception);
                        loggingService.LogResult(failedStatsUploadResult);
                        statsUploadEmailService.SendEmail(failedStatsUploadResult);
                        statsUploadDataStoreService.StatsUploadError(failedStatsUploadResult);
                        statsUploadResults.Add(failedStatsUploadResult);
                    }
                }

                return new StatsUploadResults();
            }
            catch (Exception exception)
            {
                loggingService.LogException(exception);
                return new StatsUploadResults(FailedReason.UnexpectedException);
            }
        }

        private bool DataStoreUnavailable()
        {
            return !statsUploadDataStoreService.IsAvailable();
        }

        private StatsUploadResult NewFailedStatsUploadResult(Exception exception)
        {
            if (exception is InvalidStatsFileException)
            {
                return new StatsUploadResult();
            }

            throw exception;
        }
    }
}