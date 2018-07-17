namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class FoldingUser
    {
        public FoldingUser(string friendlyName, string bitcoinAddress, long pointsGained, long workUnitsGained)
        {
            FriendlyName = friendlyName;
            BitcoinAddress = bitcoinAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public string BitcoinAddress { get; }

        public string FriendlyName { get; set; }

        public long PointsGained { get; }

        public long WorkUnitsGained { get; }
    }
}