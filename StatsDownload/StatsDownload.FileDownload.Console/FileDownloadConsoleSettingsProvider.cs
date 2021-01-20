namespace StatsDownload.FileDownload.Console
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core.Interfaces;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class FileDownloadConsoleSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
                                                       IDataStoreSettings, IStatsFileDateTimeFormatsAndOffsetSettings,
                                                       IAzureDataStoreSettingsService
    {
        public string ConnectionString => GetConnectionString("FoldingCoin.Storage");

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

        public string GetMinimumWaitTimeInHours()
        {
            return GetAppSetting("MinimumWaitTimeInHours");
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