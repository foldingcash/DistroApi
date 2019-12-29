namespace StatsDownloadApi.WebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;

    using NLog;
    using NLog.Web;

    using StatsDownloadApi.WebApi.CastleWindsor;

    public class Program
    {
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>()
                          .ConfigureLogging(logger => { logger.ClearProviders(); }).UseNLog().Build();
        }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
            WindsorContainer.Dispose();
            LogManager.Shutdown();
        }
    }
}