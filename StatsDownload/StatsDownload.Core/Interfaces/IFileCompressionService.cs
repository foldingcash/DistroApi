namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFileCompressionService
    {
        void DecompressFile(FilePayload filePayload);
    }
}