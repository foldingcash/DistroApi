namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFileReaderService
    {
        void ReadFile(FilePayload filePayload);
    }
}