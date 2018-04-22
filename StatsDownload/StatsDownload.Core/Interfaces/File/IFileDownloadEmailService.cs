namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFileDownloadEmailService
    {
        void SendEmail(FileDownloadResult fileDownloadResult);
    }
}