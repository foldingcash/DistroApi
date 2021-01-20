namespace StatsDownload.TestHarness
{
    using System.Configuration;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class TestHarnessSettingsProvider : ITestHarnessSettingsService, ITestHarnessStatsDownloadSettings
    {
        bool ITestHarnessStatsDownloadSettings.Enabled => GetBoolConfig("EnableSqlExceptionDuringAddUsersTest");

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