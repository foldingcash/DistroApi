namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class DistroUser
    {
        public DistroUser(string bitcoinAddress, long pointsGained, long workUnitsGained)
        {
            BitcoinAddress = bitcoinAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public decimal Amount { get; set; }

        public string BitcoinAddress { get; }

        public long PointsGained { get; }

        public long WorkUnitsGained { get; }
    }
}