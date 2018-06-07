namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IResourceCleanupService
    {
        void Cleanup(FileDownloadResult fileDownloadResult);
    }
}