namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFileDownloadEmailService
    {
        void SendEmail(FileDownloadResult fileDownloadResult);
    }
}