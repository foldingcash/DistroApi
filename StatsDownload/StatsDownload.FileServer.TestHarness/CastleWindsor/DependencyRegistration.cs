namespace StatsDownload.FileServer.TestHarness
{
    using System;
    using System.IO;
    using System.Reflection;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor.Installer;

    internal static class DependencyRegistration
    {
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        internal static void Register()
        {
            WindsorContainer.Instance.Install(FromAssembly.InDirectory(new AssemblyFilter(AssemblyDirectory)));
        }
    }
}