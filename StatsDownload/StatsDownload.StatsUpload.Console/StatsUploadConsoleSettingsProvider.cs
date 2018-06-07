namespace StatsDownload.StatsUpload.Console
{
    using System;
    using System.Configuration;
    using Core.Interfaces;
    using Email;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class StatsUploadConsoleSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
        IEmailSettingsService, IZeroPointUsersFilterSettings, IGoogleUsersFilterSettings,
        IWhitespaceNameUsersFilterSettings, INoPaymentAddressUsersFilterSettings
    {
        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"]?.ConnectionString;
        }

        public string GetAcceptAnySslCert()
        {
            return ConfigurationManager.AppSettings["AcceptAnySslCert"];
        }

        public string GetDownloadDirectory()
        {
            return ConfigurationManager.AppSettings["DownloadDirectory"]
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory,
                       Environment.SpecialFolderOption.Create);
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