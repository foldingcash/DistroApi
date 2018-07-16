namespace StatsDownloadApi.Core
{
    using System.Collections.Generic;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        public IList<DistroUser> GetDistro(int amount, IList<FoldingUser> foldingUsers)
        {
            var distro = new List<DistroUser>();

            foreach (FoldingUser foldingUser in foldingUsers)
            {
                distro.Add(new DistroUser(foldingUser.BitcoinAddress, foldingUser.PointsGained,
                    foldingUser.WorkUnitsGained, 0));
            }

            return distro;
        }
    }
}