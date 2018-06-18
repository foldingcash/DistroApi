namespace StatsDownload.FileDownload.Console
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using Core.Interfaces;
    using Email;

    /// <summary>
    ///     These app setting names are NOT in with the rest of the constants because they should NEVER be used elsewhere.
    /// </summary>
    public class FileDownloadConsoleSettingsProvider : IDatabaseConnectionSettingsService, IDownloadSettingsService,
        IEmailSettingsService
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
    }
}