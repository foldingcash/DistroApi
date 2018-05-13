namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.DataTransfer;

    public interface IStatsUploadService
    {
        StatsUploadResults UploadStatsFiles();
    }
}