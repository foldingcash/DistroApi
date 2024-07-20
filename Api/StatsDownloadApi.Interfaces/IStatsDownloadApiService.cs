namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IStatsDownloadApiService
    {
        Task<GetDistroResponse> GetDistro(DateTime? startDate, DateTime? endDate, int? amount, FoldingUserTypes includeFoldingUserTypes);

        Task<GetMembersResponse> GetMembers(DateTime? startDate, DateTime? endDate);

        Task<GetTeamsResponse> GetTeams();
    }
}