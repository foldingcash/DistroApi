namespace StatsDownload.Core
{
    public interface IDownloadService
    {
        void DownloadFile(StatsPayload statsPayload);
    }
}