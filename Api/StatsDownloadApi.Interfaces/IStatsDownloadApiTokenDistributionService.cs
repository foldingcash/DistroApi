namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public interface IStatsDownloadApiTokenDistributionService
    {
        IList<DistroUser> GetDistro(IList<DistroUser> distro);
    }
}