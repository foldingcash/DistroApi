namespace StatsDownloadApi.Core
{
    public class DistroUser
    {
        public DistroUser(string bitcoinAddress)
        {
            BitcoinAddress = bitcoinAddress;
        }

        public string BitcoinAddress { get; }
    }
}