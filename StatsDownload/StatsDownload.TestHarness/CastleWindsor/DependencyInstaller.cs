namespace StatsDownload.TestHarness
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using StatsDownload.Core;
    using StatsDownload.SharpZipLib;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IGuidService>().ImplementedBy<GuidProvider>(),
                Component.For<IFileDeleteService>().ImplementedBy<FileDeleteProvider>(),
                Component.For<IResourceCleanupService>().ImplementedBy<ResourceCleanupProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<ILoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component.For<IDatabaseConnectionSettingsService, IDownloadSettingsService>()
                    .ImplementedBy<TestHarnessSettingsProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<SqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDataStoreService>().ImplementedBy<FileDownloadDataStoreProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<SecureHttpFilePayloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<SecureDownloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IFileDownloadService>().ImplementedBy<FileDownloadProvider>());
        }
    }
}