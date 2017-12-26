namespace StatsDownload.Core
{
    public interface IFileNameService
    {
        string GetFileDownloadPath(string directory);

        string GetUncompressedFileDownloadPath(string directory);
    }
}