namespace StatsDownload.Core
{
    using System;

    public class StatsUploadProvider : IStatsUploadService
    {
        private readonly IStatsUploadLoggingService loggingService;

        private readonly IStatsUploadDataStoreService statsUploadDataStoreService;

        public StatsUploadProvider(IStatsUploadDataStoreService statsUploadDataStoreService,
                                   IStatsUploadLoggingService loggingService)
        {
            this.statsUploadDataStoreService = statsUploadDataStoreService;
            this.loggingService = loggingService;
        }

        public StatsUploadResult UploadStatsFile()
        {
            try
            {
                loggingService.LogVerbose($"{nameof(UploadStatsFile)} Invoked");
                if (DataStoreUnavailable())
                {
                    return new StatsUploadResult(FailedReason.DataStoreUnavailable);
                }

                return new StatsUploadResult();
            }
            catch (Exception ex)
            {
                loggingService.LogException(ex);
                return new StatsUploadResult(FailedReason.UnexpectedException);
            }
        }

        private bool DataStoreUnavailable()
        {
            return !statsUploadDataStoreService.IsAvailable();
        }
    }
}