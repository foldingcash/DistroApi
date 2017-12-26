namespace StatsDownload.Core
{
    public interface IFileDownloaderService
    {
        void DownloadFile(StatsPayload statsPayload);
    }
}