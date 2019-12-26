namespace StatsDownload.FileDownload.Console
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Email;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class FileDownloadConsoleSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
                                                       IEmailSettingsService, IDataStoreSettings,
                                                       IStatsFileDateTimeFormatsAndOffsetSettings,
                                                       IAzureDataStoreSettingsService
    {
        public string ConnectionString => GetConnectionString("FoldingCoin.DataStore");

        public string ContainerName => GetAppSetting("AzureDataStore.ContainerName");

        public string DataStoreType => GetAppSetting("DataStoreType");

        public string UploadDirectory => GetAppSetting("UploadDirectory");

        public string GetAcceptAnySslCert()
        {
            return GetAppSetting("AcceptAnySslCert");
        }

        public int? GetCommandTimeout()
        {
            string commandTimeoutString = GetAppSetting("DbCommandTimeout");
            if (int.TryParse(commandTimeoutString, out int commandTimeoutValue))
            {
                return commandTimeoutValue;
            }

            return null;
        }

        public string GetConnectionString()
        {
            return GetConnectionString("FoldingCoin.Database");
        }

        public string GetDatabaseType()
        {
            return GetAppSetting("DatabaseType");
        }

        public string GetDownloadDirectory()
        {
            return GetAppSetting("DownloadDirectory")
                   ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string GetDownloadTimeout()
        {
            return GetAppSetting("DownloadTimeoutSeconds");
        }

        public string GetDownloadUri()
        {
            return GetAppSetting("DownloadUri");
        }

        public string GetFromAddress()
        {
            return GetAppSetting("FromAddress");
        }

        public string GetFromDisplayName()
        {
            return GetAppSetting("DisplayName");
        }

        public string GetMinimumWaitTimeInHours()
        {
            return GetAppSetting("MinimumWaitTimeInHours");
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

        public string GetStatsFileTimeZoneAndOffsetSettings()
        {
            return GetAppSetting("StatsFileTimeZoneAndOffset");
        }

        private string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key]?.ConnectionString;
        }
    }
}