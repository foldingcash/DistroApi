namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IStatsDownloadApiDataStoreService
    {
        Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate, FoldingUserTypes includeFoldingUserTypes);

        Task<Member[]> GetMembers(DateTime startDate, DateTime endDate);

        Task<Team[]> GetTeams();

        Task<bool> IsAvailable();
    }
}