namespace StatsDownloadApi.Interfaces
{
    using System;

    public interface IStatsDownloadApiService
    {
        GetDistroResponse GetDistro(DateTime? startDate, DateTime? endDate, int? amount);

        GetTeamsResponse GetTeams();
    }
}