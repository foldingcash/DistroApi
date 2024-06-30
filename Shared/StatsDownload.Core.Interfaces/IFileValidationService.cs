namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileValidationService
    {
        void PreValidateFile(FilePayload filePayload);
        ParseResults ValidateFile(FilePayload filePayload);
    }
}