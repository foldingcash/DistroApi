namespace StatsDownload.Core.Interfaces
{
    using Interfaces.DataTransfer;

    public interface IFileDownloadMinimumWaitTimeService
    {
        bool IsMinimumWaitTimeMet(FilePayload filePayload);
    }
}