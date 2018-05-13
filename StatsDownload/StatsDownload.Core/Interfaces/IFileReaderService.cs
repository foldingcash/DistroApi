namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFileReaderService
    {
        void ReadFile(FilePayload filePayload);
    }
}