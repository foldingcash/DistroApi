namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IResourceCleanupService
    {
        void Cleanup(FileDownloadResult fileDownloadResult);
    }
}