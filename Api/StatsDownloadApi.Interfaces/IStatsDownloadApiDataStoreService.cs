namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IStatsDownloadApiDataStoreService
    {
        Task<FoldingUser[]> GetFoldingMembers(DateTime startDate, DateTime endDate);

        Task<Member[]> GetMembers(DateTime startDate, DateTime endDate);

        Task<Team[]> GetTeams();

        Task<bool> IsAvailable();
    }
}