namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core.Interfaces;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class TestHarnessSettingsProvider : ITestHarnessSettingsService, IZeroPointUsersFilterSettings,
                                               IGoogleUsersFilterSettings, IWhitespaceNameUsersFilterSettings,
                                               ITestHarnessStatsDownloadSettings
    {
        bool IGoogleUsersFilterSettings.Enabled => GetBoolConfig("EnableGoogleUsersFilter");

        bool ITestHarnessStatsDownloadSettings.Enabled => GetBoolConfig("EnableSqlExceptionDuringAddUsersTest");

        bool IWhitespaceNameUsersFilterSettings.Enabled => GetBoolConfig("EnableWhitespaceNameUsersFilter");

        bool IZeroPointUsersFilterSettings.Enabled => GetBoolConfig("EnableZeroPointUsersFilter");
        
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