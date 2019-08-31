namespace StatsDownload.TestHarness.CastleWindsor
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Implementations.Filters;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Database;
    using StatsDownload.Database.CastleWindsor;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.SharpZipLib;
    using StatsDownload.Wrappers;
    using StatsDownload.Wrappers.Networking;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IApplicationLoggingService>().ImplementedBy<TestHarnessLoggingProvider>(),
                Component
                    .For<IDatabaseConnectionSettingsService, IDownloadSettingsService, ITestHarnessSettingsService,
                        IEmailSettingsService, ITestHarnessStatsDownloadSettings>()
                    .ImplementedBy<TestHarnessSettingsProvider>()
                    .Forward<IZeroPointUsersFilterSettings, IGoogleUsersFilterSettings,
                        IWhitespaceNameUsersFilterSettings, INoPaymentAddressUsersFilterSettings>()
                    .Forward<IStatsFileDateTimeFormatsAndOffsetSettings, IDataStoreSettings>(),
                Component.For<IFileDownloadMinimumWaitTimeService>()
                         .ImplementedBy<TestHarnessMinimumWaitTimeProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<TestHarnessSecureHttpFilePayloadProvider>(),
                Component.For<IStatsFileParserService>().ImplementedBy<TestHarnessOneHundredUsersFilter>(),
                Component.For<IStatsUploadDatabaseService>().ImplementedBy<TestHarnessStatsUploadDatabaseProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<TestHarnessFileCompressionProvider>(),
                Component.For<ISelectExportFilesProvider>().ImplementedBy<SelectExportFilesForm>());

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
                Component.For<IStatsDownloadDatabaseParameterService>()
                         .ImplementedBy<StatsDownloadDatabaseParameterProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .IsDefault(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DatabaseFactoryComponentSelector>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DatabaseFactoryComponentSelector>()),
                Component.For<IStatsDownloadDatabaseService>().ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<IFileDownloadDatabaseService>().ImplementedBy<FileDownloadDatabaseProvider>(),
                Component.For<IStatsUploadDatabaseService>().ImplementedBy<StatsUploadDatabaseProvider>(),
                Component.For<ISecureFilePayloadService>().ImplementedBy<SecureFilePayloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<SecureDownloadProvider>(),
                Component.For<IDownloadService>().ImplementedBy<DownloadProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IStatsUploadService>().ImplementedBy<StatsUploadProvider>(),
                Component.For<IStatsFileDateTimeFormatsAndOffsetService>()
                         .ImplementedBy<StatsFileDateTimeFormatsAndOffsetProvider>(),
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
                Component.For<IStatsDownloadEmailService, IFileDownloadEmailService, IStatsUploadEmailService>()
                         .ImplementedBy<StatsDownloadEmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IFilePayloadUploadService>().ImplementedBy<FilePayloadUploadProvider>(),
                Component.For<IWebClient>().ImplementedBy<WebClientWrapper>().LifestyleTransient(),
                Component.For<IWebClientFactory>().AsFactory(),
                Component.For<IDataStoreService>().ImplementedBy<UncDataStoreProvider>(),
                Component.For<IFileValidationService>().ImplementedBy<FileValidationProvider>());
        }
    }
}