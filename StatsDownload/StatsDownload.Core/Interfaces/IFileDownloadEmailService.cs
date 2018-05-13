namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFileDownloadEmailService
    {
        void SendEmail(FileDownloadResult fileDownloadResult);
    }
}