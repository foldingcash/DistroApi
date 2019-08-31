namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileValidationService
    {
        void ValidateFile(FilePayload filePayload);
    }
}