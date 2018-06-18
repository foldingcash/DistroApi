namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IStatsUploadService
    {
        StatsUploadResults UploadStatsFiles();
    }
}