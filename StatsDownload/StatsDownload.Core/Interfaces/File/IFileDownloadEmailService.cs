namespace StatsDownload.Core
{
    public interface IFileDownloadEmailService
    {
        void SendEmail(FileDownloadResult fileDownloadResult);
    }
}