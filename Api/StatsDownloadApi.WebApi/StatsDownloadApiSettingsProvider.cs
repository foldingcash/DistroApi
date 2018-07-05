namespace StatsDownloadApi.WebApi
{
    using Microsoft.Extensions.Configuration;
    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApiSettingsProvider : IDatabaseConnectionSettingsService
    {
        private readonly IConfiguration configuration;

        public StatsDownloadApiSettingsProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public int? GetCommandTimeout()
        {
            string commandTimeoutString = GetAppSetting("DbCommandTimeout");
            int commandTimeoutValue;
            if (int.TryParse(commandTimeoutString, out commandTimeoutValue))
                return commandTimeoutValue;
            return null;
        }

        public string GetConnectionString()
        {
            return configuration.GetConnectionString("FoldingCoin");
        }

        public string GetDatabaseType()
        {
            return GetAppSetting("DatabaseType");
        }

        private string GetAppSetting(string name)
        {
            return configuration.GetSection("AppSettings").GetValue<string>(name);
        }
    }
}