namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFileDownloadService
    {
        FileDownloadResult DownloadStatsFile();
    }
}