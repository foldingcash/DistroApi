namespace StatsDownload.Core
{
    public interface IFileDownloadDataStoreService
    {
        void FileDownloadFinished(StatsPayload statsPayload);

        bool IsAvailable();

        StatsPayload NewFileDownloadStarted();

        void UpdateToLatest();
    }
}