namespace StatsDownloadApi.Interfaces
{
    using System;

    public interface IStatsDownloadApiService
    {
        GetDistroResponse GetDistro(DateTime? startDate, DateTime? endDate, int? amount);

        GetMemberStatsResponse GetMemberStats(DateTime? startDate, DateTime? endDate);

        GetTeamsResponse GetTeams();
    }
}