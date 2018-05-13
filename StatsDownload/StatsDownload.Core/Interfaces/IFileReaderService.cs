namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFileReaderService
    {
        void ReadFile(FilePayload filePayload);
    }
}