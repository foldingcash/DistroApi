namespace StatsDownload.Core
{
    using Interfaces;

    public interface IFileDownloadMinimumWaitTimeService
    {
        bool IsMinimumWaitTimeMet(FilePayload filePayload);
    }
}