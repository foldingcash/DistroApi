namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class DistroUser
    {
        public DistroUser(string bitcoinAddress, long pointsGained, long workUnitsGained,
            decimal amount)
        {
            BitcoinAddress = bitcoinAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
            Amount = amount;
        }

        public decimal Amount { get; set; }

        public string BitcoinAddress { get; }

        public long PointsGained { get; }

        public long WorkUnitsGained { get; }
    }
}