namespace StatsDownloadApi.WebApi
{
    using System.IO;
    using System.Reflection;

    using Microsoft.Extensions.Configuration;

    using StatsDownload.Core.Interfaces;

    public class StatsDownloadApiSettingsProvider : IDownloadSettingsService
    {
        private readonly IConfiguration configuration;

        public StatsDownloadApiSettingsProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
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

        private string GetAppSetting(string name)
        {
            return configuration.GetSection("AppSettings").GetValue<string>(name);
        }
    }
}