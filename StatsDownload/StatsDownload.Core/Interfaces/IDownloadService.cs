namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IDownloadService
    {
        void DownloadFile(FilePayload filePayload);
    }
}