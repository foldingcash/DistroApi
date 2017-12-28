namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core;

    public class TestHarnessSettingsProvider : IDatabaseConnectionSettingsService, IFileDownloadSettingsService
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"].ConnectionString;
        }

        public string GetDownloadDirectory()
        {
            return ConfigurationManager.AppSettings["DownloadDirectory"]
                   ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string GetDownloadTimeout()
        {
            return ConfigurationManager.AppSettings["DownloadTimeoutSeconds"] ?? "100";
        }

        public string GetDownloadUri()
        {
            return ConfigurationManager.AppSettings["DownloadUri"];
        }
    }
}