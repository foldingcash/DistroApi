namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

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
                services.AddStatsDownload(context.Configuration,
                    (innerService, provider) => new TestHarnessOneHundredUsersFilter(innerService,
                        provider.GetRequiredService<IOptions<TestHarnessSettings>>()),
                    (innerService, provider) => new TestHarnessMinimumWaitTimeProvider(innerService,
                        provider.GetRequiredService<IOptions<TestHarnessSettings>>()),
                    (innerService, provider) => new TestHarnessSecureHttpFilePayloadProvider(innerService,
                        provider.GetRequiredService<IOptions<TestHarnessSettings>>()),
                    (innerService, provider) => new TestHarnessFileCompressionProvider(innerService,
                        provider.GetRequiredService<IOptions<TestHarnessSettings>>()));

                services.AddSingleton<MainForm>().AddSingleton<ISelectExportFilesProvider, SelectExportFilesForm>()
                        .AddSingleton<IApplicationLoggingService, TestHarnessLoggingService>();

                
            }).ConfigureLogging(builder =>
            {
                builder.AddProvider(new TestHarnessLoggerProvider());
            });

            IHost host = hostBuilder.Build();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = host.Services.GetRequiredService<MainForm>();
            var logger = new LogWriter(mainForm.LoggingTextBox);
            Console.SetOut(logger);
            Application.Run(mainForm);
        }
    }
}