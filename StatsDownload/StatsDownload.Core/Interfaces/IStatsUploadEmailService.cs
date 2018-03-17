namespace StatsDownload.Core
{
    public interface IStatsUploadEmailService
    {
        void SendEmail(StatsUploadResult statsUploadResult);
    }
}