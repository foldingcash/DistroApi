namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFileDownloadEmailService
    {
        void SendEmail(FileDownloadResult fileDownloadResult);
    }
}