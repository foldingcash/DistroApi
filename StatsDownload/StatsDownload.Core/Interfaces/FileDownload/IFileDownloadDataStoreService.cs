namespace StatsDownload.Core
{
    public interface IFileDownloadDataStoreService
    {
        void FileDownloadFinished(FilePayload filePayload);

        bool IsAvailable();

        void NewFileDownloadStarted(FilePayload filePayload);

        void UpdateToLatest();
    }
}