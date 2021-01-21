namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using StatsDownload.Core.TestHarness.CastleWindsor;
    using StatsDownload.DependencyInjection;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.ConfigureStatsDownload(context.Configuration);

                services.AddSingleton<MainForm>();
            });

            IHost host = hostBuilder.Build();

            DependencyRegistration.Register();
            Application.ApplicationExit += (sender, args) => WindsorContainer.Dispose();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(host.Services.GetRequiredService<MainForm>());
        }
    }
}