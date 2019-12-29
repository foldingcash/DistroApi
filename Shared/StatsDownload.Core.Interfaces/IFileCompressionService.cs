namespace StatsDownload.Core.Interfaces
{
    public interface IFileCompressionService
    {
        void CompressFile(string filePath, string compressedFilePath);

        void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath);
    }
}