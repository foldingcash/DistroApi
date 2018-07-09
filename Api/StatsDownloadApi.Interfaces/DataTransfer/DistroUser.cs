namespace StatsDownloadApi.Interfaces.DataTransfer
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