namespace StatsDownloadApi.WebApi.CastleWindsor
{
    using System;

    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using NLog;
    using NLog.Web;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Database;
    using StatsDownload.Database.CastleWindsor;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.DataStore;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.Parsing;
    using StatsDownload.Parsing.Filters;
    using StatsDownload.SharpZipLib;
    using StatsDownload.Wrappers;

    using StatsDownloadApi.Core;
    using StatsDownloadApi.Database;
    using StatsDownloadApi.DataStore;
    using StatsDownloadApi.Interfaces;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>().Instance(CreateLogger()));

            container.Register(
                Component
                    .For<IDatabaseConnectionSettingsService, IEmailSettingsService, IDownloadSettingsService,
                        IDataStoreSettings, IStatsFileDateTimeFormatsAndOffsetSettings>()
                    .Forward<INoPaymentAddressUsersFilterSettings, IAzureDataStoreSettingsService>().ImplementedBy<StatsDownloadApiSettingsProvider>(),
                Component.For<IApplicationLoggingService>().ImplementedBy<StatsDownloadApiLoggingProvider>(),
                Component.For<IStatsDownloadApiEmailService>().ImplementedBy<StatsDownloadApiEmailProvider>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IErrorMessageService>().ImplementedBy<ErrorMessageProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .IsDefault(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DatabaseFactoryComponentSelector>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DatabaseFactoryComponentSelector>()),
                Component.For<IStatsDownloadApiTokenDistributionService>()
                         .ImplementedBy<StandardTokenDistributionProvider>(),
                Component.For<IStatsDownloadDatabaseService>().ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<IStatsDownloadApiDatabaseService>()
                         .ImplementedBy<StatsDownloadApiDatabaseCacheProvider>(),
                Component.For<IStatsDownloadApiDatabaseService>()
                         .ImplementedBy<StatsDownloadApiDatabaseValidationProvider>(),
                Component.For<IStatsDownloadApiDatabaseService>().ImplementedBy<StatsDownloadApiDatabaseProvider>(),
                Component.For<IStatsDownloadApiService>().ImplementedBy<StatsDownloadApiProvider>(),
                Component.For<IStatsDownloadApiDataStoreService>()
                         .ImplementedBy<StatsDownloadApiDataStoreCacheProvider>(),
                Component.For<IStatsDownloadApiDataStoreService>().ImplementedBy<StatsDownloadApiDataStoreProvider>(),
                Component.For<IDataStoreService>().ImplementedBy<AzureDataStoreProvider>(),
                Component.For<IDataStoreService>().ImplementedBy<UncDataStoreProvider>(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DataStoreFactoryComponentSelector>(),
                Component.For<IDataStoreServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DataStoreFactoryComponentSelector>()),
                Component.For<IFileService>().ImplementedBy<FileProvider>(),
                Component.For<IFileValidationService>().ImplementedBy<FileValidationProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IFilePayloadApiSettingsService>().ImplementedBy<FilePayloadApiSettingsProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IDownloadSettingsValidatorService>().ImplementedBy<DownloadSettingsValidatorProvider>(),
                Component.For<IDirectoryService>().ImplementedBy<DirectoryProvider>());

            container.Register(Component.For<IStatsFileParserService>().ImplementedBy<NoPaymentAddressUsersFilter>(),
                Component.For<IStatsFileParserService>().ImplementedBy<StatsFileParserProvider>(),
                Component.For<IAdditionalUserDataParserService>().ImplementedBy<AdditionalUserDataParserProvider>(),
                Component.For<IBitcoinAddressValidatorService>().ImplementedBy<BitcoinAddressValidatorProvider>(),
                Component.For<IStatsFileDateTimeFormatsAndOffsetService>()
                         .ImplementedBy<StatsFileDateTimeFormatsAndOffsetProvider>());
        }

        private ILogger CreateLogger()
        {
            try
            {
                return NLogBuilder.ConfigureNLog("nlog.statsapi.config").GetCurrentClassLogger();
            }
            catch (Exception)
            {
                return LogManager.CreateNullLogger();
            }
        }
    }
}