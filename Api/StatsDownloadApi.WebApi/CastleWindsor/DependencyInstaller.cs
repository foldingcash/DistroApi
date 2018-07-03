namespace StatsDownloadApi.WebApi.CastleWindsor
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IStatsDownloadApi>().ImplementedBy<StatsDownloadApi>().LifestylePerThread());
        }
    }
}