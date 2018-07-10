namespace StatsDownloadApi.Interfaces
{
    using System;

    public interface IStatsDownloadApiService
    {
        DistroResponse GetDistro(DateTime? startDate, DateTime? endDate);
    }
}