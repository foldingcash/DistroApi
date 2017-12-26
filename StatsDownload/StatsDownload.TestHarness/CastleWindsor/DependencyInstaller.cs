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
                Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IGuidService>().ImplementedBy<GuidProvider>(),
                Component.For<IFileNameService>().ImplementedBy<FileNameProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IFileDownloadLoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component.For<IDatabaseConnectionSettingsService, IFileDownloadSettingsService>()
                    .ImplementedBy<TestHarnessSettingsProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<SqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDataStoreService>().ImplementedBy<FileDownloadDataStoreProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IFileDownloadTimeoutValidatorService>()
                    .ImplementedBy<FileDownloadTimeoutValidatorProvider>(),
                Component.For<IFileDownloadService>().ImplementedBy<FileDownloadProvider>());
        }
    }
}