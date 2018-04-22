namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFileCompressionService
    {
        void DecompressFile(FilePayload filePayload);
    }
}