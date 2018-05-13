namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IResourceCleanupService
    {
        void Cleanup(FileDownloadResult fileDownloadResult);
    }
}