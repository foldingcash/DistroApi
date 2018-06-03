namespace StatsDownload.Core.Interfaces
{
    public interface IStatsDownloadDatabaseService : IFileDownloadDatabaseService, IStatsUploadDatabaseService
    {
        new bool IsAvailable();
    }
}