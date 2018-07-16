namespace StatsDownloadApi.Core
{
    using System.Collections.Generic;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        public IList<DistroUser> GetDistro(int amount, IList<DistroUser> distro)
        {
            return distro;
        }
    }
}