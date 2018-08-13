namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        private const int MaxPrecision = 8;

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

        public decimal Round(decimal value, int precision)
        {
            return Math.Round(value, precision, MidpointRounding.ToEven);
        }

        private decimal GetAmount(int amount, long totalPoints, FoldingUser foldingUser)
        {
            decimal rawAmount = Convert.ToDecimal(foldingUser.PointsGained) / Convert.ToDecimal(totalPoints) *
                                Convert.ToDecimal(amount);

            return Round(rawAmount, MaxPrecision);
        }

        private DistroUser GetDistroUser(int amount, long totalPoints, FoldingUser foldingMember)
        {
            return new DistroUser(foldingMember.BitcoinAddress, foldingMember.PointsGained,
                foldingMember.WorkUnitsGained, GetAmount(amount, totalPoints, foldingMember));
        }
    }
}