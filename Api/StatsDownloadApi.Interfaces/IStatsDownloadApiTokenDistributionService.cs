namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiTokenDistributionService
    {
        IList<DistroUser> GetDistro(int amount, IList<DistroUser> distro);
    }
}