namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFileReaderService
    {
        void ReadFile(FilePayload filePayload);
    }
}