namespace StatsDownload.FileDownload.Console
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    using StatsDownload.Core.Interfaces;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class FileDownloadConsoleSettingsProvider : IDownloadSettingsService
    {
        public string GetAcceptAnySslCert()
        {
            return GetAppSetting("AcceptAnySslCert");
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
        
        private string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}