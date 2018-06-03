namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}