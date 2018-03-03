namespace StatsDownload.Core
{
    public interface IDataStoreService : IFileDownloadDataStoreService, IStatsUploadDataStoreService
    {
        new bool IsAvailable();
    }
}