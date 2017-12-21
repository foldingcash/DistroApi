namespace StatsDownload.Core
{
    public interface IFileDownloaderService
    {
        void DownloadFile(string address, string fileName, int timeoutInSeconds);
    }
}