namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}