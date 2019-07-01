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
                                                       IEmailSettingsService
    {
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

        public string GetFromAddress()
        {
            return ConfigurationManager.AppSettings["FromAddress"];
        }

        public string GetFromDisplayName()
        {
            return ConfigurationManager.AppSettings["DisplayName"];
        }

        public string GetMinimumWaitTimeInHours()
        {
            return ConfigurationManager.AppSettings["MinimumWaitTimeInHours"];
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
    }
}