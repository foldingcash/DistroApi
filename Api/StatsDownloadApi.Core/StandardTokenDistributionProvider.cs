namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        public IList<DistroUser> GetDistro(int amount, IList<FoldingUser> foldingUsers)
        {
            var distro = new List<DistroUser>();

            long totalPoints = foldingUsers.Sum(user => user.PointsGained);

            foreach (FoldingUser foldingUser in foldingUsers)
            {
                distro.Add(GetDistroUser(amount, totalPoints, foldingUser));
            }

            return distro;
        }

        private decimal GetAmount(int amount, long totalPoints, FoldingUser foldingUser)
        {
            return (Convert.ToDecimal(foldingUser.PointsGained) / Convert.ToDecimal(totalPoints)) *
                   Convert.ToDecimal(amount);
        }

        private DistroUser GetDistroUser(int amount, long totalPoints, FoldingUser foldingUser)
        {
            return new DistroUser(foldingUser.BitcoinAddress, foldingUser.PointsGained,
                foldingUser.WorkUnitsGained, GetAmount(amount, totalPoints, foldingUser));
        }
    }
}