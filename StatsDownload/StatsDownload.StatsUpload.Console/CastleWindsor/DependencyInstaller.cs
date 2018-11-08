namespace StatsDownload.StatsUpload.Console.CastleWindsor
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core.Implementations;
    using Core.Interfaces;
    using Core.Interfaces.Logging;
    using Database;
    using Database.CastleWindsor;
    using Database.Wrappers;
    using Email;
    using Logging;
    using NLog;
    using SharpZipLib;
    using Wrappers;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>()
                                        .Instance(LogManager
                                                  .LoadConfiguration("nlog.statsupload.config")
                                                  .GetCurrentClassLogger()));

            container.Register(
                Component.For<IApplicationLoggingService>().ImplementedBy<StatsUploadConsoleLoggingProvider>(),
                Component.For<IDatabaseConnectionSettingsService, IDownloadSettingsService, IEmailSettingsService>()
                         .ImplementedBy<StatsUploadConsoleSettingsProvider>()
                         .Forward<IZeroPointUsersFilterSettings, IGoogleUsersFilterSettings,
                             IWhitespaceNameUsersFilterSettings, INoPaymentAddressUsersFilterSettings>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IFileService>().ImplementedBy<FileProvider>(),
                Component.For<IDirectoryService>().ImplementedBy<DirectoryProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IStatsUploadLoggingService>().ImplementedBy<StatsDownloadLoggingProvider>(),
                Component.For<IFilePayloadSettingsService>().ImplementedBy<FilePayloadSettingsProvider>(),
                Component.For<IFileCompressionService>().ImplementedBy<Bz2CompressionProvider>(),
                Component.For<IFileReaderService>().ImplementedBy<FileReaderProvider>(),
                Component.For<IStatsDownloadDatabaseParameterService>()
                         .ImplementedBy<StatsDownloadDatabaseParameterProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MySqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .IsDefault(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DatabaseFactoryComponentSelector>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DatabaseFactoryComponentSelector>()),
                Component.For<IStatsDownloadDatabaseService>().ImplementedBy<StatsDownloadDatabaseProvider>(),
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
                Component.For<IErrorMessageService>().ImplementedBy<ErrorMessageProvider>(),
                Component.For<IStatsUploadEmailService>().ImplementedBy<StatsDownloadEmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IFilePayloadUploadService>().ImplementedBy<FilePayloadUploadProvider>());
        }
    }
}