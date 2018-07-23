namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IFileDownloadMinimumWaitTimeService
    {
        bool IsMinimumWaitTimeMet(FilePayload filePayload);
    }
}