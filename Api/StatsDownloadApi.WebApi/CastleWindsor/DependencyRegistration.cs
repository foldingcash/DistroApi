namespace StatsDownloadApi.WebApi.CastleWindsor
{
    internal static class DependencyRegistration
    {
        internal static void Register()
        {
            WindsorContainer.Instance.Install(new DependencyInstaller());
        }
    }
}