namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core.Interfaces;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class TestHarnessSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
                                               ITestHarnessSettingsService, IZeroPointUsersFilterSettings,
                                               IGoogleUsersFilterSettings, IWhitespaceNameUsersFilterSettings,
                                               INoPaymentAddressUsersFilterSettings, ITestHarnessStatsDownloadSettings,
                                               IStatsFileDateTimeFormatsAndOffsetSettings, IDataStoreSettings,
                                               IAzureDataStoreSettingsService
    {
        public string ConnectionString =>
            ConfigurationManager.ConnectionStrings["FoldingCoin.Storage"].ConnectionString;

        public string ContainerName => ConfigurationManager.AppSettings["AzureDataStore.ContainerName"];

        public string DataStoreType => ConfigurationManager.AppSettings["DataStoreType"];

        bool IGoogleUsersFilterSettings.Enabled => GetBoolConfig("EnableGoogleUsersFilter");

        bool INoPaymentAddressUsersFilterSettings.Enabled => GetBoolConfig("EnableNoPaymentAddressUsersFilter");

        bool ITestHarnessStatsDownloadSettings.Enabled => GetBoolConfig("EnableSqlExceptionDuringAddUsersTest");

        bool IWhitespaceNameUsersFilterSettings.Enabled => GetBoolConfig("EnableWhitespaceNameUsersFilter");

        bool IZeroPointUsersFilterSettings.Enabled => GetBoolConfig("EnableZeroPointUsersFilter");

        public string UploadDirectory => ConfigurationManager.AppSettings["UploadDirectory"];

        public string GetAcceptAnySslCert()
        {
            return ConfigurationManager.AppSettings["AcceptAnySslCert"];
        }

        public int? GetCommandTimeout()
        {
            string commandTimeoutString = ConfigurationManager.AppSettings["DbCommandTimeout"];
            if (int.TryParse(commandTimeoutString, out int commandTimeoutValue))
            {
                return commandTimeoutValue;
            }

            return null;
        }

        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin.Database"]?.ConnectionString;
        }

        public string GetDatabaseType()
        {
            return ConfigurationManager.AppSettings["DatabaseType"];
        }

        public string GetDownloadDirectory()
        {
            return ConfigurationManager.AppSettings["DownloadDirectory"]
                   ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string GetDownloadTimeout()
        {
            return ConfigurationManager.AppSettings["DownloadTimeoutSeconds"];
        }

        public string GetDownloadUri()
        {
            return ConfigurationManager.AppSettings["DownloadUri"];
        }

        public string GetMinimumWaitTimeInHours()
        {
            return ConfigurationManager.AppSettings["MinimumWaitTimeInHours"];
        }

        public string GetStatsFileTimeZoneAndOffsetSettings()
        {
            return ConfigurationManager.AppSettings["StatsFileTimeZoneAndOffset"];
        }

        public bool IsFileCompressionDisabled()
        {
            return GetBoolConfig("FileCompressionDisabled");
        }

        public bool IsMinimumWaitTimeMetDisabled()
        {
            return GetBoolConfig("DisableMinimumWaitTime");
        }

        public bool IsOneHundredUsersFilterEnabled()
        {
            return GetBoolConfig("EnableOneHundredUsersFilter");
        }

        public bool IsSecureFilePayloadDisabled()
        {
            return GetBoolConfig("DisableSecureFilePayload");
        }

        private bool GetBoolConfig(string appSettingName)
        {
            bool.TryParse(ConfigurationManager.AppSettings[appSettingName], out bool value);
            return value;
        }
    }
}