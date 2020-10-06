namespace StatsDownload.FileDownload.Console
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider provider = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.ConfigureThisApp();
            }).Build().Services;

            var service = provider.GetService<IFileDownloadService>();

            FileDownloadResult results = await service.DownloadStatsFile();

            Environment.Exit(results.Success ? 0 : -1);
        }
    }
}