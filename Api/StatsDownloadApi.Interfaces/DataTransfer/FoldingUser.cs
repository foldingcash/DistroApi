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

        public string PaymentAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(BitcoinAddress))
                {
                    return BitcoinAddress;
                }
                if (!string.IsNullOrEmpty(BitcoinCashAddress))
                {
                    return BitcoinCashAddress;
                }
                if (!string.IsNullOrEmpty(SlpAddress))
                {
                    return SlpAddress;
                }
                if (!string.IsNullOrEmpty(CashTokensAddress))
                {
                    return CashTokensAddress;
                }
                return string.Empty;
            }
        }
    }
}