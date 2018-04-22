namespace StatsDownload.Core
{
    using Interfaces;

    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}