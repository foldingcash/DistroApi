namespace StatsDownload.Core.TestHarness.CastleWindsor
{
    internal static class DependencyRegistration
    {
        internal static void Register()
        {
            WindsorContainer.Instance.Install(new DependencyInstaller());
        }
    }
}