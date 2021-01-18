namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        private const int MaxPrecision = 8;

        public IList<DistroUser> GetDistro(int amount, IList<FoldingUser> foldingUsers)
        {
            var distro = new List<DistroUser>();

            AddDistroUsers(amount, distro, foldingUsers);

            return distro;
        }

        public decimal Round(decimal value, int precision)
        {
            return Math.Round(value, precision, MidpointRounding.ToEven);
        }

        private void AddDistroUsers(int amount, List<DistroUser> distro, IList<FoldingUser> foldingUsers)
        {
            long totalPoints = GetTotalPoints(foldingUsers);

            foreach (FoldingUser foldingUser in foldingUsers)
            {
                AddingUserToDistro(amount, distro, totalPoints, foldingUser);
            }
        }

        private void AddingUserToDistro(int amount, List<DistroUser> distro, long totalPoints, FoldingUser foldingUser)
        {
            if (distro.Exists(user => user.BitcoinAddress == foldingUser.BitcoinAddress))
            {
                DistroUser previousUser = distro.Find(user => user.BitcoinAddress == foldingUser.BitcoinAddress);
                DistroUser combinedUser = NewDistroUser(amount, totalPoints, foldingUser.BitcoinAddress,
                    previousUser.PointsGained + foldingUser.PointsGained,
                    previousUser.WorkUnitsGained + foldingUser.WorkUnitsGained);
                distro.Remove(previousUser);
                distro.Add(combinedUser);
            }
            else
            {
                DistroUser distroUser = NewDistroUser(amount, totalPoints, foldingUser.BitcoinAddress,
                    foldingUser.PointsGained, foldingUser.WorkUnitsGained);
                distro.Add(distroUser);
            }
        }

        private decimal GetRewardAmount(int amount, long totalPoints, long pointsGained)
        {
            if (totalPoints == 0)
            {
                throw new InvalidDistributionStateException(
                    "The total points earned was zero. A distribution cannot happen when zero points are earned because the distribution would be zero. Enter a new start date and/or end date and try again. If the error continues or you think this is incorrect, then contact your support team with this response.");
            }

            decimal rawAmount = Convert.ToDecimal(pointsGained) / Convert.ToDecimal(totalPoints)
                                * Convert.ToDecimal(amount);

            return Round(rawAmount, MaxPrecision);
        }

        private long GetTotalPoints(IList<FoldingUser> foldingUsers)
        {
            return foldingUsers.Sum(user => user.PointsGained);
        }

        private DistroUser NewDistroUser(int amount, long totalPoints, string bitcoinAddress, long pointsGained,
                                         long workUnitsGained)
        {
            return new DistroUser(bitcoinAddress, pointsGained, workUnitsGained,
                GetRewardAmount(amount, totalPoints, pointsGained));
        }
    }
}