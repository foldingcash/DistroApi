namespace StatsDownload.FileDownload.Console.CastleWindsor
{
    internal static class DependencyRegistration
    {
        internal static void Register()
        {
            WindsorContainer.Instance.Install(new DependencyInstaller());
        }
    }
}