namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public interface IStatsDownloadApiTokenDistributionService
    {
        IList<DistroUser> GetDistro(int amount, IList<FoldingUser> foldingUsers);
    }
}