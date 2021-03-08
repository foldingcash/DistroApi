namespace StatsDownload.DependencyInjection
{
    using System;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

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
                                                          IConfiguration configuration,
                                                          Func<IStatsFileParserService, IServiceProvider,
                                                              IStatsFileParserService> decorateStatsFileParserService,
                                                          Func<IFileDownloadMinimumWaitTimeService, IServiceProvider,
                                                                  IFileDownloadMinimumWaitTimeService>
                                                              decorateFileDownloadMinimumWaiteTimeService,
                                                          Func<ISecureFilePayloadService, IServiceProvider,
                                                                  ISecureFilePayloadService>
                                                              decorateSecureFilePayloadService,
                                                          Func<IFileCompressionService, IServiceProvider,
                                                              IFileCompressionService> decorateFileCompressionService)
        {
            serviceCollection.AddEmail(configuration).AddStatsDownloadSettings(configuration).AddDatabase()
                             .AddDataStore().AddWebClient();

            serviceCollection.AddSingleton<IDateTimeService, DateTimeProvider>()
                             .AddSingleton<IGuidService, GuidProvider>().AddSingleton<IFileService, FileProvider>()
                             .AddSingleton<IDirectoryService, DirectoryProvider>()
                             .AddSingleton<IResourceCleanupService, ResourceCleanupProvider>()
                             .AddSingleton<ILoggingService, LoggingProvider>()
                             .AddSingleton<IFileDownloadLoggingService, StatsDownloadLoggingProvider>()
                             .AddSingleton<IStatsUploadLoggingService, StatsDownloadLoggingProvider>()
                             .AddSingleton<IFilePayloadSettingsService, FilePayloadSettingsProvider>()
                             .AddSingleton<IFileReaderService, FileReaderProvider>()
                             .AddSingleton<IStatsDownloadDatabaseParameterService,
                                 StatsDownloadDatabaseParameterProvider>()
                             .AddSingleton<IStatsDownloadDatabaseService, StatsDownloadDatabaseProvider>()
                             .AddSingleton<IFileDownloadDatabaseService, FileDownloadDatabaseProvider>()
                             .AddSingleton<IDownloadSettingsValidatorService, DownloadSettingsValidatorProvider>()
                             .AddSingleton<IStatsFileDateTimeFormatsAndOffsetService,
                                 StatsFileDateTimeFormatsAndOffsetProvider>()
                             .AddSingleton<IAdditionalUserDataParserService, AdditionalUserDataParserProvider>()
                             .AddSingleton<IBitcoinAddressValidatorService, BitcoinAddressValidatorProvider>()
                             .AddSingleton<IBitcoinCashAddressValidatorService, BitcoinCashAddressValidatorProvider>()
                             .AddSingleton<ISlpAddressValidatorService, SlpAddressValidatorProvider>()
                             .AddSingleton<IFileDownloadService, FileDownloadProvider>()
                             .AddSingleton<IFileDownloadMinimumWaitTimeService, FileDownloadMinimumWaitTimeProvider>()
                             .AddSingleton<IErrorMessageService, ErrorMessageProvider>()
                             .AddSingleton<IStatsDownloadEmailService, StatsDownloadEmailProvider>()
                             .AddSingleton<IFileDownloadEmailService, StatsDownloadEmailProvider>()
                             .AddSingleton<IFilePayloadUploadService, FilePayloadUploadProvider>()
                             .AddSingleton<IFileValidationService, FileValidationProvider>();

            serviceCollection.AddSingleton(provider =>
            {
                var service = new SecureFilePayloadProvider(provider.GetRequiredService<ILoggingService>());
                return decorateSecureFilePayloadService?.Invoke(service, provider) ?? service;
            });

            serviceCollection.AddSingleton<IDownloadService>(provider =>
            {
                var service = new SecureDownloadProvider(provider.GetRequiredService<ILogger<SecureDownloadProvider>>(),
                    new DownloadProvider(provider.GetRequiredService<ILogger<DownloadProvider>>(),
                        provider.GetRequiredService<IDateTimeService>(),
                        provider.GetRequiredService<IWebClientFactory>()),
                    provider.GetRequiredService<ISecureFilePayloadService>());
                return service;
            });

            serviceCollection.AddSingleton(provider =>
            {
                var service = new GoogleUsersFilter(
                    new NoPaymentAddressUsersFilter(
                        new WhitespaceNameUsersFilter(
                            new ZeroPointUsersFilter(
                                new StatsFileParserProvider(
                                    provider.GetRequiredService<ILogger<StatsFileParserProvider>>(),
                                    provider.GetRequiredService<IAdditionalUserDataParserService>(),
                                    provider.GetRequiredService<IStatsFileDateTimeFormatsAndOffsetService>()),
                                provider.GetRequiredService<IOptions<FilterSettings>>()),
                            provider.GetRequiredService<IOptions<FilterSettings>>()),
                        provider.GetRequiredService<IOptions<FilterSettings>>()),
                    provider.GetRequiredService<IOptions<FilterSettings>>());

                return decorateStatsFileParserService?.Invoke(service, provider) ?? service;
            });

            serviceCollection.AddSingleton(provider =>
            {
                var service = new FileDownloadMinimumWaitTimeProvider(
                    provider.GetRequiredService<IFileDownloadDatabaseService>(),
                    provider.GetRequiredService<IDateTimeService>());
                return decorateFileDownloadMinimumWaiteTimeService?.Invoke(service, provider) ?? service;
            });

            serviceCollection.AddSingleton(provider =>
            {
                var service =
                    new Bz2CompressionProvider(provider.GetRequiredService<ILogger<Bz2CompressionProvider>>());
                return decorateFileCompressionService?.Invoke(service, provider) ?? service;
            });

            return serviceCollection;
        }

        public static IServiceCollection AddStatsDownload(this IServiceCollection serviceCollection,
                                                          IConfiguration configuration)
        {
            return serviceCollection.AddStatsDownload(configuration, null, null, null, null);
        }

        public static IServiceCollection AddStatsDownloadSettings(this IServiceCollection serviceCollection,
                                                                  IConfiguration configuration)
        {
            return serviceCollection.AddOptions()
                                    .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<FilterSettings>(configuration.GetSection(nameof(FilterSettings)))
                                    .Configure<DateTimeSettings>(configuration.GetSection(nameof(DateTimeSettings)))
                                    .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<DataStoreSettings>(configuration.GetSection(nameof(DataStoreSettings)))
                                    .Configure<DownloadSettings>(configuration.GetSection(nameof(DownloadSettings)));
        }

        private static IServiceCollection AddDatabase(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IDatabaseConnectionServiceFactory, DatabaseConnectionServiceFactory>()
                                    .AddSingleton<MicrosoftSqlDatabaseConnectionProvider>();
        }

        private static IServiceCollection AddDataStore(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IDataStoreServiceFactory, DataStoreServiceFactory>()
                                    .AddSingleton<UncDataStoreProvider>();
        }

        private static IServiceCollection AddWebClient(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<IWebClient, WebClientWrapper>()
                                    .AddSingleton<IWebClientFactory, WebClientFactory>();
        }
    }
}