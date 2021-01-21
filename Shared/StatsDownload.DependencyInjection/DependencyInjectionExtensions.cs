namespace StatsDownload.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.Database;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.DataStore;
    using StatsDownload.Email.DependencyInjection;
    using StatsDownload.Logging;
    using StatsDownload.Parsing;
    using StatsDownload.Parsing.Filters;
    using StatsDownload.SharpZipLib;
    using StatsDownload.Wrappers;
    using StatsDownload.Wrappers.Networking;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddStatsDownload(this IServiceCollection serviceCollection,
                                                          IConfiguration configuration)
        {
            return serviceCollection.AddEmail(configuration).AddStatsDownloadSettings(configuration)
                                    .AddSingleton<IDateTimeService, DateTimeProvider>()
                                    .AddSingleton<IFileService, FileProvider>()
                                    .AddSingleton<IDirectoryService, DirectoryProvider>()
                                    .AddSingleton<IResourceCleanupService, ResourceCleanupProvider>()
                                    .AddSingleton<ILoggingService, LoggingProvider>()
                                    .AddSingleton<IFileDownloadLoggingService, StatsDownloadLoggingProvider>()
                                    .AddSingleton<IStatsUploadLoggingService, StatsDownloadLoggingProvider>()
                                    .AddSingleton<IFilePayloadSettingsService, FilePayloadSettingsProvider>()
                                    .AddSingleton<IFileCompressionService, Bz2CompressionProvider>()
                                    .AddSingleton<IFileReaderService, FileReaderProvider>()
                                    .AddSingleton<IStatsDownloadDatabaseParameterService,
                                        StatsDownloadDatabaseParameterProvider>()
                                    .AddSingleton<IDatabaseConnectionService, MicrosoftSqlDatabaseConnectionProvider>()
                                    .AddSingleton<IStatsDownloadDatabaseService, StatsDownloadDatabaseProvider>()
                                    .AddSingleton<IFileDownloadDatabaseService, FileDownloadDatabaseProvider>()
                                    .AddSingleton<ISecureFilePayloadService, SecureFilePayloadProvider>()
                                    .AddSingleton<IDownloadService, SecureDownloadProvider>()
                                    .AddSingleton<IDownloadService, DownloadProvider>()
                                    .AddSingleton<IDownloadSettingsValidatorService, DownloadSettingsValidatorProvider
                                    >().AddSingleton<IStatsFileDateTimeFormatsAndOffsetService,
                                        StatsFileDateTimeFormatsAndOffsetProvider>()
                                    .AddSingleton<IStatsFileParserService, GoogleUsersFilter>()
                                    .AddSingleton<IStatsFileParserService, NoPaymentAddressUsersFilter>()
                                    .AddSingleton<IStatsFileParserService, WhitespaceNameUsersFilter>()
                                    .AddSingleton<IStatsFileParserService, ZeroPointUsersFilter>()
                                    .AddSingleton<IStatsFileParserService, StatsFileParserProvider>()
                                    .AddSingleton<IAdditionalUserDataParserService, AdditionalUserDataParserProvider>()
                                    .AddSingleton<IBitcoinAddressValidatorService, BitcoinAddressValidatorProvider>()
                                    .AddSingleton<IFileDownloadService, FileDownloadProvider>()
                                    .AddSingleton<IFileDownloadMinimumWaitTimeService,
                                        FileDownloadMinimumWaitTimeProvider>()
                                    .AddSingleton<IErrorMessageService, ErrorMessageProvider>()
                                    .AddSingleton<IStatsDownloadEmailService, StatsDownloadEmailProvider>()
                                    .AddSingleton<IFileDownloadEmailService, StatsDownloadEmailProvider>()
                                    .AddSingleton<IFilePayloadUploadService, FilePayloadUploadProvider>()
                                    .AddTransient<IWebClient, WebClientWrapper>()
                                    .AddSingleton<IFileValidationService, FileValidationProvider>()
                                    .AddSingleton<IDataStoreService, UncDataStoreProvider>();
        }

        public static IServiceCollection AddStatsDownloadSettings(this IServiceCollection serviceCollection,
                                                                  IConfiguration configuration)
        {
            return serviceCollection.AddOptions()
                                    .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<FilterSettings>(configuration.GetSection(nameof(FilterSettings)))
                                    .Configure<DateTimeSettings>(configuration.GetSection(nameof(DateTimeSettings)))
                                    .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<DataStoreSettings>(configuration.GetSection(nameof(DataStoreSettings)));
        }
    }
}