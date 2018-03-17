namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;

    public class StatsUploadProvider : IStatsUploadService
    {
        private readonly IStatsUploadLoggingService loggingService;

        private readonly IStatsFileParserService statsFileParserService;

        private readonly IStatsUploadDataStoreService statsUploadDataStoreService;

        public StatsUploadProvider(IStatsUploadDataStoreService statsUploadDataStoreService,
                                   IStatsUploadLoggingService loggingService,
                                   IStatsFileParserService statsFileParserService)
        {
            this.statsUploadDataStoreService = statsUploadDataStoreService;
            this.loggingService = loggingService;
            this.statsFileParserService = statsFileParserService;
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

                foreach (int uploadFile in uploadFiles)
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
    }
}