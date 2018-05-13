namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface IFileDownloadMinimumWaitTimeService
    {
        bool IsMinimumWaitTimeMet(FilePayload filePayload);
    }
}