namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class FoldingUser
    {
        public FoldingUser(string friendlyName, string bitcoinAddress, string bitcoinCashAddress, string slpAddress,
            string cashTokensAddress, long pointsGained, long workUnitsGained)
        {
            FriendlyName = friendlyName;
            BitcoinAddress = bitcoinAddress;
            BitcoinCashAddress = bitcoinCashAddress;
            SlpAddress = slpAddress;
            CashTokensAddress = cashTokensAddress;
            PointsGained = pointsGained;
            WorkUnitsGained = workUnitsGained;
        }

        public string BitcoinAddress { get; }

        public string BitcoinCashAddress { get; set; }

        public string CashTokensAddress { get; set; }

        public string FriendlyName { get; set; }

        public long PointsGained { get; }

        public string SlpAddress { get; set; }

        public long WorkUnitsGained { get; }
    }
}