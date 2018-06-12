namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Core.Interfaces;
    using Email;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class TestHarnessSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
        ITestHarnessSettingsService, IEmailSettingsService, IZeroPointUsersFilterSettings, IGoogleUsersFilterSettings,
        IWhitespaceNameUsersFilterSettings, INoPaymentAddressUsersFilterSettings
    {
        public int? GetCommandTimeout()
        {
            string commandTimeoutString = ConfigurationManager.AppSettings["DbCommandTimeout"];
            int commandTimeoutValue;
            if (int.TryParse(commandTimeoutString, out commandTimeoutValue))
                return commandTimeoutValue;
            return null;
        }

        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"]?.ConnectionString;
        }

        public string GetDatabaseType()
        {
            return ConfigurationManager.AppSettings["DatabaseType"];
        }

        public string GetAcceptAnySslCert()
        {
            return ConfigurationManager.AppSettings["AcceptAnySslCert"];
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

        public string GetFromAddress()
        {
            return ConfigurationManager.AppSettings["FromAddress"];
        }

        public string GetFromDisplayName()
        {
            return ConfigurationManager.AppSettings["DisplayName"];
        }

        public string GetPassword()
        {
            return ConfigurationManager.AppSettings["Password"];
        }

        public string GetPort()
        {
            return ConfigurationManager.AppSettings["Port"];
        }

        public string GetReceivers()
        {
            return ConfigurationManager.AppSettings["Receivers"];
        }

        public string GetSmtpHost()
        {
            return ConfigurationManager.AppSettings["SmtpHost"];
        }

        bool IGoogleUsersFilterSettings.Enabled => GetBoolConfig("EnableGoogleUsersFilter");

        bool INoPaymentAddressUsersFilterSettings.Enabled => GetBoolConfig("EnableNoPaymentAddressUsersFilter");

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

        bool IWhitespaceNameUsersFilterSettings.Enabled => GetBoolConfig("EnableWhitespaceNameUsersFilter");

        bool IZeroPointUsersFilterSettings.Enabled => GetBoolConfig("EnableZeroPointUsersFilter");

        private bool GetBoolConfig(string appSettingName)
        {
            bool value;
            bool.TryParse(ConfigurationManager.AppSettings[appSettingName], out value);
            return value;
        }
    }
}