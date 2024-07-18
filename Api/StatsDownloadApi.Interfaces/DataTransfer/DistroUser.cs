namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class DistroUser
    {
        public DistroUser(string bitcoinAddress, string bitcoinCashAddress, string slpAddress, string cashTokensAddress,
            long pointsGained, long workUnitsGained, decimal amount)
        {
            BitcoinAddress = bitcoinAddress;
            BitcoinCashAddress = bitcoinCashAddress;
            SlpAddress = slpAddress;
            CashTokensAddress = cashTokensAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
            Amount = amount;
        }

        public decimal Amount { get; set; }

        public string BitcoinAddress { get; }

        public string BitcoinCashAddress { get; set; }

        public string CashTokensAddress { get; set; }

        public long PointsGained { get; }

        public string SlpAddress { get; set; }

        public long WorkUnitsGained { get; }
    }
}