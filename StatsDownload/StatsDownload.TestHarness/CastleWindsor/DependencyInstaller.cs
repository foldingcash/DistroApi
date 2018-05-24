namespace StatsDownload.TestHarness
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Implementations.Untested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Core.Wrappers.Networking;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.SharpZipLib;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IApplicationLoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component
                    .For
                    <IDatabaseConnectionSettingsService, IDownloadSettingsService, ITestHarnessSettingsService,
                        IEmailSettingsService>().ImplementedBy<TestHarnessSettingsProvider>(),
                Component.For<IFileDownloadMinimumWaitTimeService>().ImplementedBy<TestHarnessMinimumWaitTimeProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<TestHarnessSecureHttpFilePayloadProvider>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IGuidService>().ImplementedBy<GuidProvider>(),
                Component.For<IFileService>().ImplementedBy<FileProvider>(),
                Component.For<IDirectoryService>().ImplementedBy<DirectoryProvider>(),
                Component.For<IResourceCleanupService>().ImplementedBy<ResourceCleanupProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IFileDownloadLoggingService, IStatsUploadLoggingService>()
                         .ImplementedBy<StatsDownloadLoggingProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IDatabaseConnectionService>()
                         .ImplementedBy<SqlDatabaseConnectionProvider>()
                         .LifestyleSingleton(), Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDatabaseService, IStatsUploadDatabaseService>()
                         .ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<SecureFilePayloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<SecureDownloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IStatsUploadService>().ImplementedBy<StatsUploadProvider>(),
                Component.For<IStatsFileParserService>().ImplementedBy<StatsFileParserProvider>(),
                Component.For<IAdditionalUserDataParserService>().ImplementedBy<AdditionalUserDataParserProvider>(),
                Component.For<IBitcoinAddressValidatorService>().ImplementedBy<BitcoinAddressValidatorProvider>(),
                Component.For<IFileDownloadService>().ImplementedBy<FileDownloadProvider>(),
                Component.For<IFileDownloadMinimumWaitTimeService>()
                         .ImplementedBy<FileDownloadMinimumWaitTimeProvider>(),
                Component.For<IErrorMessageService>().ImplementedBy<ErrorMessageProvider>(),
                Component.For<IFileDownloadEmailService, IStatsUploadEmailService>()
                         .ImplementedBy<StatsDownloadEmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IFilePayloadUploadService>().ImplementedBy<FilePayloadUploadProvider>(),
                Component.For<IHttpClient>().ImplementedBy<HttpClientWrapper>().LifestyleTransient(),
                Component.For<IHttpClientFactory>().AsFactory());
        }
    }
}