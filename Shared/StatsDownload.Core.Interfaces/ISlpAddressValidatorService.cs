namespace StatsDownload.Core.Interfaces
{
    public interface ISlpAddressValidatorService
    {
        bool IsValidSlpAddress(string address);
    }
}