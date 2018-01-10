namespace StatsDownload.TestHarness
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core;

    public class TestHarnessSettingsProvider : IDatabaseConnectionSettingsService,
                                               IDownloadSettingsService,
                                               ITestHarnessSettingsService
    {
        public string GetAcceptAnySslCert()
        {
            return ConfigurationManager.AppSettings["AcceptAnySslCert"];
        }

        public string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FoldingCoin"].ConnectionString;
        }

        public string GetDownloadDirectory()
        {
            return ConfigurationManager.AppSettings["DownloadDirectory"]
                   ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string GetDownloadTimeout()
        {
            return ConfigurationManager.AppSettings["DownloadTimeoutSeconds"] ?? "100";
        }

        public string GetDownloadUri()
        {
            return ConfigurationManager.AppSettings["DownloadUri"];
        }

        public string GetMinimumWaitTimeInHours()
        {
            return ConfigurationManager.AppSettings["MinimumWaitTimeInHours"];
        }

        public bool IsMinimumWaitTimeMetDisabled()
        {
            bool disableMinimumWaitTime;
            bool.TryParse(ConfigurationManager.AppSettings["DisableMinimumWaitTime"], out disableMinimumWaitTime);
            return disableMinimumWaitTime;
        }

        public bool IsSecureFilePayloadDisabled()
        {
            bool disableSecureFilePayload;
            bool.TryParse(ConfigurationManager.AppSettings["DisableSecureFilePayload"], out disableSecureFilePayload);
            return disableSecureFilePayload;
        }
    }
}