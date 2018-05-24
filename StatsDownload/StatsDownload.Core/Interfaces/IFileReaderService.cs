namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileReaderService
    {
        void ReadFile(FilePayload filePayload);
    }
}