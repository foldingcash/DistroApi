namespace StatsDownload.FileDownload.Console
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.DependencyInjection;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider provider = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.AddStatsDownload(context.Configuration);
                services.AddSingleton<IApplicationLoggingService, FileDownloadConsoleLoggingProvider>();
            }).Build().Services;

            var service = provider.GetRequiredService<IFileDownloadService>();

            FileDownloadResult results = await service.DownloadStatsFile();

            Environment.Exit(results.Success ? 0 : -1);
        }
    }
}