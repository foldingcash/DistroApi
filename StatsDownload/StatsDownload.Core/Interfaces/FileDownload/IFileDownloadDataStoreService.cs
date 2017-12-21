namespace StatsDownload.Core
{
    public interface IFileDownloadDataStoreService
    {
        bool IsAvailable();

        int NewFileDownloadStarted();

        void UpdateToLatest();
    }
}