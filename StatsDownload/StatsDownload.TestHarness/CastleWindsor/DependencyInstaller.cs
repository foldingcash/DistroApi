namespace StatsDownload.TestHarness.CastleWindsor
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core.Implementations;
    using Core.Interfaces;
    using Core.Interfaces.Logging;
    using Core.Interfaces.Networking;
    using Core.Wrappers;
    using Core.Wrappers.Networking;
    using Email;
    using Logging;
    using SharpZipLib;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IApplicationLoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component
                    .For<IDatabaseConnectionSettingsService, IDownloadSettingsService, ITestHarnessSettingsService,
                        IEmailSettingsService>().ImplementedBy<TestHarnessSettingsProvider>()
                    .Forward<IZeroPointUsersFilterSettings, IGoogleUsersFilterSettings,
                        IWhitespaceNameUsersFilterSettings, INoPaymentAddressUsersFilterSettings>(),
                Component.For<IFileDownloadMinimumWaitTimeService>()
                         .ImplementedBy<TestHarnessMinimumWaitTimeProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<TestHarnessSecureHttpFilePayloadProvider>(),
                Component.For<IStatsFileParserService>().ImplementedBy<TestHarnessOneHundredUsersFilter>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IFileService>().ImplementedBy<FileProvider>(),
                Component.For<IDirectoryService>().ImplementedBy<DirectoryProvider>(),
                Component.For<IResourceCleanupService>().ImplementedBy<ResourceCleanupProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IFileDownloadLoggingService, IStatsUploadLoggingService>()
                         .ImplementedBy<StatsDownloadLoggingProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .LifestyleSingleton(), Component.For<IDatabaseConnectionServiceFactory>().AsFactory(),
                Component.For<IFileDownloadDatabaseService, IStatsUploadDatabaseService>()
                         .ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<SecureFilePayloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<SecureDownloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IStatsUploadService>().ImplementedBy<StatsUploadProvider>(),
                Component.For<IStatsFileParserService>().ImplementedBy<GoogleUsersFilter>(),
                Component.For<IStatsFileParserService>().ImplementedBy<NoPaymentAddressUsersFilter>(),
                Component.For<IStatsFileParserService>().ImplementedBy<WhitespaceNameUsersFilter>(),
                Component.For<IStatsFileParserService>().ImplementedBy<ZeroPointUsersFilter>(),
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
                Component.For<IWebClient>().ImplementedBy<WebClientWrapper>().LifestyleTransient(),
                Component.For<IWebClientFactory>().AsFactory());
        }
    }
}