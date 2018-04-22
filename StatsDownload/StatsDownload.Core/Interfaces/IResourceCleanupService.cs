namespace StatsDownload.Core
{
    using Interfaces;

    public interface IResourceCleanupService
    {
        void Cleanup(FileDownloadResult fileDownloadResult);
    }
}