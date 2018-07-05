namespace StatsDownloadApi.Core
{
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApi : IStatsDownloadApi
    {
        private readonly IStatsDownloadDatabaseService databaseService;

        public StatsDownloadApi(IStatsDownloadDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public DistroResponse GetDistro()
        {
            if (!databaseService.IsAvailable())
            {
                return new DistroResponse(false);
            }

            return new DistroResponse(true);
        }
    }
}