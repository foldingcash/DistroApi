namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}