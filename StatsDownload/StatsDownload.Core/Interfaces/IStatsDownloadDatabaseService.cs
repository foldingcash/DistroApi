namespace StatsDownload.Core
{
    public interface IStatsDownloadDatabaseService : IFileDownloadDatabaseService, IStatsUploadDatabaseService
    {
        new bool IsAvailable();
    }
}