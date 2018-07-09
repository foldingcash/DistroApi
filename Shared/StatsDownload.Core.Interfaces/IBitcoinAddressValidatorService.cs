namespace StatsDownload.Core.Interfaces
{
    public interface IBitcoinAddressValidatorService
    {
        bool IsValidBitcoinAddress(string bitcoinAddress);
    }
}