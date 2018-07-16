namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class FoldingUser
    {
        public FoldingUser(string bitcoinAddress, long pointsGained, long workUnitsGained)
        {
            BitcoinAddress = bitcoinAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public string BitcoinAddress { get; }

        public long PointsGained { get; }

        public long WorkUnitsGained { get; }
    }
}