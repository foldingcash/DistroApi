namespace StatsDownload.TestHarness
{
    using System.Configuration;

    using StatsDownload.Core;

    public class TestHarnessSettingsProvider : IDatabaseConnectionSettingsService, IFileDownloadSettingsService
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"].ConnectionString;
        }

        public string GetDownloadDirectory()
        {
            return ConfigurationManager.AppSettings["DownloadDirectory"];
        }

        public string GetDownloadTimeout()
        {
            return ConfigurationManager.AppSettings["DownloadTimeout"];
        }

        public string GetDownloadUrl()
        {
            return ConfigurationManager.AppSettings["DownloadUrl"];
        }
    }
}