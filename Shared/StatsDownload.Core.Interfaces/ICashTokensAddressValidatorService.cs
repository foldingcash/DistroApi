namespace StatsDownload.Core.Interfaces
{
    public interface ICashTokensAddressValidatorService
    {
        string GetBitcoinAddress(string address);

        bool IsValidCashTokensAddress(string address);
    }
}