namespace StatsDownload.Core.Interfaces
{
    public interface IBitcoinCashAddressValidatorService
    {
        string GetBitcoinAddress(string address);

        bool IsValidBitcoinCashAddress(string address);
    }
}