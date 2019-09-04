namespace StatsDownloadApi.WebApi
{
    using CastleWindsor;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Web;

    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureLogging(logger =>
                   {
                       logger.ClearProviders();
                   })
                   .UseNLog()
                   .Build();

        public static void Main(string[] args)
        {
            DependencyRegistration.Register();

            BuildWebHost(args).Run();

            WindsorContainer.Dispose();

            LogManager.Shutdown();
        }
    }
}