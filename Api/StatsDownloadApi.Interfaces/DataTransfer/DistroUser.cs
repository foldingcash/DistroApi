namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class DistroUser
    {
        public DistroUser(string friendlyName, string bitcoinAddress, long pointsGained, long workUnitsGained,
            decimal amount)
        {
            FriendlyName = friendlyName;
            BitcoinAddress = bitcoinAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
            Amount = amount;
        }

        public decimal Amount { get; set; }

        public string BitcoinAddress { get; }

        public string FriendlyName { get; set; }

        public long PointsGained { get; }

        public long WorkUnitsGained { get; }
    }
}