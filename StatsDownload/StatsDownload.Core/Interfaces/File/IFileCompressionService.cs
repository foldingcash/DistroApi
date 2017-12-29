namespace StatsDownload.Core
{
    public interface IFileCompressionService
    {
        void DecompressFile(FilePayload filePayload);
    }
}