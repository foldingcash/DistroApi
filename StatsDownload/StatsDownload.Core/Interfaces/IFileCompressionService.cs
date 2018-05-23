namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFileCompressionService
    {
        void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath);
    }
}