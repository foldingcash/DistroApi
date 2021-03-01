namespace StatsDownload.Core.Interfaces
{
    public interface ISlpAddressValidatorService
    {
        string GetBitcoinAddress(string address);

        bool IsValidSlpAddress(string address);
    }
}