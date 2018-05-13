namespace StatsDownload.Core.Interfaces
{
    public interface IDownloadSettingsService
    {
        string GetAcceptAnySslCert();

        string GetDownloadDirectory();

        string GetDownloadTimeout();

        string GetDownloadUri();

        string GetMinimumWaitTimeInHours();
    }
}