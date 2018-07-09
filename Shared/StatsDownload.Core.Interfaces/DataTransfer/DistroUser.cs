namespace StatsDownload.Core.Interfaces.DataTransfer
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