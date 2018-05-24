namespace StatsDownload.Core.Interfaces
{
    public interface IFileCompressionService
    {
        void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath);
    }
}