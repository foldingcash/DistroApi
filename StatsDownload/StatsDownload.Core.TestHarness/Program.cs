namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.DependencyInjection;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddStatsDownload(context.Configuration);

                services.AddSingleton<IApplicationLoggingService, TestHarnessLoggingProvider>()
                        .AddSingleton<IFileDownloadMinimumWaitTimeService, TestHarnessMinimumWaitTimeProvider>()
                        .AddSingleton<ISecureFilePayloadService, TestHarnessSecureHttpFilePayloadProvider>()
                        .AddSingleton<IStatsFileParserService, TestHarnessOneHundredUsersFilter>()
                        .AddSingleton<IStatsUploadDatabaseService, TestHarnessStatsUploadDatabaseProvider>()
                        .AddSingleton<IFileCompressionService, TestHarnessFileCompressionProvider>()
                        .AddSingleton<ISelectExportFilesProvider, SelectExportFilesForm>().AddSingleton<MainForm>();
            });

            IHost host = hostBuilder.Build();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(host.Services.GetRequiredService<MainForm>());
        }
    }
}