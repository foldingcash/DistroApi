namespace StatsDownload.Core
{
    public interface IFileDownloadSettingsService
    {
        string GetDownloadDirectory();

        string GetDownloadTimeout();

        string GetDownloadUrl();
    }
}