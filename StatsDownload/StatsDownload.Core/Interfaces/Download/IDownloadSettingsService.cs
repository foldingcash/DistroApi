namespace StatsDownload.Core
{
    public interface IDownloadSettingsService
    {
        string GetAcceptAnySslCert();

        string GetDownloadDirectory();

        string GetDownloadTimeout();

        string GetDownloadUri();
    }
}