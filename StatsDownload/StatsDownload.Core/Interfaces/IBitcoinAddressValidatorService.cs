namespace StatsDownload.Core
{
    public interface IBitcoinAddressValidatorService
    {
        bool IsValidBitcoinAddress(string bitcoinAddress);
    }
}