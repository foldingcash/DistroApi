namespace StatsDownloadApi.WebApi
{
    using Microsoft.Extensions.Configuration;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Email;

    public class StatsDownloadApiSettingsProvider : IDatabaseConnectionSettingsService, IEmailSettingsService
    {
        private readonly IConfiguration configuration;

        public StatsDownloadApiSettingsProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public int? GetCommandTimeout()
        {
            string commandTimeoutString = GetAppSetting("DbCommandTimeout");
            if (int.TryParse(commandTimeoutString, out int commandTimeoutValue))
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

        public string GetFromAddress()
        {
            return GetAppSetting("FromAddress");
        }

        public string GetFromDisplayName()
        {
            return GetAppSetting("DisplayName");
        }

        public string GetPassword()
        {
            return GetAppSetting("Password");
        }

        public string GetPort()
        {
            return GetAppSetting("Port");
        }

        public string GetReceivers()
        {
            return GetAppSetting("Receivers");
        }

        public string GetSmtpHost()
        {
            return GetAppSetting("SmtpHost");
        }

        private string GetAppSetting(string name)
        {
            return configuration.GetSection("AppSettings").GetValue<string>(name);
        }
    }
}