namespace StatsDownload.TestHarness
{
    using System;
    using System.Windows.Forms;
    using CastleWindsor;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            DependencyRegistration.Register();
            Application.ApplicationExit += (sender, args) => WindsorContainer.Dispose();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}