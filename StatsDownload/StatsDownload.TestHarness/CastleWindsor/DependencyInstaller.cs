namespace StatsDownload.TestHarness
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using StatsDownload.Core;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.SharpZipLib;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILoggingService, IFileDownloadLoggingService>()
                         .ImplementedBy<TestHarnessLoggingProvider>(),
                Component
                    .For
                    <IDatabaseConnectionSettingsService, IDownloadSettingsService, ITestHarnessSettingsService,
                        IEmailSettingsService>().ImplementedBy<TestHarnessSettingsProvider>(),
                Component.For<IFileDownloadMinimumWaitTimeService>().ImplementedBy<TestHarnessMinimumWaitTimeProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<TestHarnessSecureHttpFilePayloadProvider>());

            container.Register(
                Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IGuidService>().ImplementedBy<GuidProvider>(),
                Component.For<IFileService>().ImplementedBy<FileProvider>(),
                Component.For<IDirectoryService>().ImplementedBy<DirectoryProvider>(),
                Component.For<IResourceCleanupService>().ImplementedBy<ResourceCleanupProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<SqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDataStoreService>().ImplementedBy<FileDownloadDataStoreProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<SecureFilePayloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<SecureDownloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IFileDownloadService>().ImplementedBy<FileDownloadProvider>(),
                Component.For<IFileDownloadMinimumWaitTimeService>()
                         .ImplementedBy<FileDownloadMinimumWaitTimeProvider>(),
                Component.For<IFileDownloadEmailService>().ImplementedBy<FileDownloadEmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IFilePayloadUploadService>().ImplementedBy<FilePayloadUploadProvider>());
        }
    }
}