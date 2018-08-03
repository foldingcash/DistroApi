namespace StatsDownloadApi.Interfaces
{
    public class MemberStats
    {
        public MemberStats(string userName, string friendlyName, string bitcoinAddress, long teamNumber,
            long pointsGained, long workUnitsGained)
        {
            UserName = userName;
            FriendlyName = friendlyName;
            BitcoinAddress = bitcoinAddress;
            TeamNumber = teamNumber;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public string BitcoinAddress { get; }

        public string FriendlyName { get; }

        public long PointsGained { get; }

        public long TeamNumber { get; }

        public string UserName { get; }

        public long WorkUnitsGained { get; }
    }
}