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

        public decimal RoundDown(decimal value, double precision)
        {
            decimal power = Convert.ToDecimal(Math.Pow(10, precision));
            return Math.Floor(value * power) / power;
        }

        private decimal GetAmount(int amount, long totalPoints, FoldingUser foldingUser)
        {
            decimal rawAmount = Convert.ToDecimal(foldingUser.PointsGained) / Convert.ToDecimal(totalPoints) *
                                Convert.ToDecimal(amount);

            return RoundDown(rawAmount, MaxPrecision);
        }

        private DistroUser GetDistroUser(int amount, long totalPoints, FoldingUser foldingUser)
        {
            return new DistroUser(foldingUser.BitcoinAddress, foldingUser.PointsGained,
                foldingUser.WorkUnitsGained, GetAmount(amount, totalPoints, foldingUser));
        }
    }
}