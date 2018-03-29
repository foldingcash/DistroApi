namespace StatsDownload.Core
{
    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}