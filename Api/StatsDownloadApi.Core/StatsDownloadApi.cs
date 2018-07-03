namespace StatsDownloadApi.Core
{
    public class StatsDownloadApi : IStatsDownloadApi
    {
        public DistroResponse GetDistro()
        {
            return new DistroResponse();
        }
    }
}