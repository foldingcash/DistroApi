namespace StatsDownload.TestHarness
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using StatsDownload.Core;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IFileDownloadLoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component.For<IDatabaseConnectionSettingsService, IFileDownloadSettingsService>()
                    .ImplementedBy<TestHarnessSettingsProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<SqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDataStoreService>().ImplementedBy<FileDownloadDataStoreProvider>(),
                Component.For<IFileDownloadService>().ImplementedBy<FileDownloadProvider>());
        }
    }
}