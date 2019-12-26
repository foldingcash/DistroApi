namespace StatsDownload.FileServer.TestHarness.CastleWindsor
{
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ITestHarnessFileServer>().ImplementedBy<TestHarnessFileServer>()
                                        .AsWcfService());
        }
    }
}