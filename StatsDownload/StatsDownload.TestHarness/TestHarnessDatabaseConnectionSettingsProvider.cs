namespace StatsDownload.TestHarness
{
    using System.Configuration;

    using StatsDownload.Core;

    public class TestHarnessDatabaseConnectionSettingsProvider : IDatabaseConnectionSettingsService
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"].ConnectionString;
        }
    }
}