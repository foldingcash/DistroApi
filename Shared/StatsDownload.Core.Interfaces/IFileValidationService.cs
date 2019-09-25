namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileValidationService
    {
        ParseResults ValidateFile(FilePayload filePayload);
    }
}