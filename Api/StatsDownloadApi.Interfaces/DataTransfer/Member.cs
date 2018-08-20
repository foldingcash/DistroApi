namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class Member
    {
        public Member(string userName, string friendlyName, string bitcoinAddress, long teamNumber, long startingPoints,
            long startingWorkUnits,
            long pointsGained, long workUnitsGained)
        {
            UserName = userName;
            FriendlyName = friendlyName;
            BitcoinAddress = bitcoinAddress;
            TeamNumber = teamNumber;
            StartingPoints = startingPoints;
            StartingWorkUnits = startingWorkUnits;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public string BitcoinAddress { get; }

        public string FriendlyName { get; }

        public long PointsGained { get; }

        public long StartingPoints { get; }

        public long StartingWorkUnits { get; }

        public long TeamNumber { get; }

        public string UserName { get; }

        public long WorkUnitsGained { get; }
    }
}