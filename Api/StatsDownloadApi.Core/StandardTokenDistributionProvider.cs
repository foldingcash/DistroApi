namespace StatsDownloadApi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StandardTokenDistributionProvider : IStatsDownloadApiTokenDistributionService
    {
        private const int DecimalPlaces = 8;

        private readonly decimal SmallestUnit = 1 / Convert.ToDecimal(Math.Pow(10, DecimalPlaces));

        public IList<DistroUser> GetDistro(int amount, IList<FoldingUser> foldingUsers)
        {
            IList<DistroUser> distro = GetDistroUsers(amount, foldingUsers);

            CorrectDrift(amount, distro);

            return distro;
        }

        public void CorrectDrift(int amount, IList<DistroUser> distro)
        {
            decimal distroAmount = distro.Sum(user => user.Amount);

            if (distroAmount > Convert.ToDecimal(amount))
            {
                IOrderedEnumerable<DistroUser> ordered = distro.OrderByDescending(user => user.PointsGained);
                DistroUser max = ordered.First();
                max.Amount -= SmallestUnit;
            }
            else if (distroAmount < Convert.ToDecimal(amount))
            {
                IOrderedEnumerable<DistroUser> ordered = distro.OrderBy(user => user.PointsGained);
                DistroUser min = ordered.First();
                min.Amount += SmallestUnit;
            }

            if (distro.Sum(user => user.Amount) != Convert.ToDecimal(amount))
            {
                CorrectDrift(amount, distro);
            }
        }

        private void AddingUserToDistro(int amount, Dictionary<string, DistroUser> distro, long totalPoints,
                                        FoldingUser foldingUser)
        {
            if (distro.ContainsKey(foldingUser.BitcoinAddress))
            {
                DistroUser previousUser = distro[foldingUser.BitcoinAddress];
                DistroUser combinedUser = NewDistroUser(amount, totalPoints, foldingUser.BitcoinAddress,
                    previousUser.PointsGained + foldingUser.PointsGained,
                    previousUser.WorkUnitsGained + foldingUser.WorkUnitsGained);
                distro[foldingUser.BitcoinAddress] = combinedUser;
            }
            else
            {
                DistroUser distroUser = NewDistroUser(amount, totalPoints, foldingUser.BitcoinAddress,
                    foldingUser.PointsGained, foldingUser.WorkUnitsGained);
                distro.Add(foldingUser.BitcoinAddress, distroUser);
            }
        }

        private IList<DistroUser> GetDistroUsers(int amount, IList<FoldingUser> foldingUsers)
        {
            var distro = new Dictionary<string, DistroUser>();
            long totalPoints = GetTotalPoints(foldingUsers);

            foreach (FoldingUser foldingUser in foldingUsers)
            {
                AddingUserToDistro(amount, distro, totalPoints, foldingUser);
            }

            return distro.Select(pair => pair.Value).ToList();
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

            return Round(rawAmount, DecimalPlaces);
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

        private decimal Round(decimal value, int precision)
        {
            return Math.Round(value, precision, MidpointRounding.ToEven);
        }
    }
}