namespace StatsDownload.Core.Interfaces
{
    public interface IStatsDownloadEmailService : IFileDownloadEmailService, IStatsUploadEmailService
    {
        void SendTestEmail();
    }
}