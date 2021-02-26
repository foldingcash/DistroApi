namespace StatsDownload.Core.Interfaces
{
    public interface IBitcoinCashAddressValidatorService
    {
        bool IsValidBitcoinCashAddress(string bitcoinCashAddress);
    }
}