namespace StatsDownload.Core.Interfaces
{
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IFileDownloadService
    {
        Task<FileDownloadResult> DownloadStatsFile();
    }
}