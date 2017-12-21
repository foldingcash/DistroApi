namespace StatsDownload.Core.Implementations
{
    public class FileDownloaderProvider : IFileDownloaderService
    {
        public void DownloadFile(string address, string fileName, int timeoutInSeconds)
        {
            using (var client = new WebClientWithTimeout())
            {
                client.TimeoutInSeconds = timeoutInSeconds;
                client.DownloadFile(address, fileName);
            }
        }
    }
}