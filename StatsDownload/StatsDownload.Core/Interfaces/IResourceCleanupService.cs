namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IResourceCleanupService
    {
        void Cleanup(FileDownloadResult fileDownloadResult);
    }
}