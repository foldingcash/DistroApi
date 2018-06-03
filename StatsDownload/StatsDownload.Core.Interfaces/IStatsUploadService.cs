namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IStatsUploadService
    {
        StatsUploadResults UploadStatsFiles();
    }
}