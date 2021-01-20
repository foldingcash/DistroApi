namespace StatsDownload.FileDownload.Console
{
    using Castle.Facilities.TypedFactory;

    using Microsoft.Extensions.DependencyInjection;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Networking;
    using StatsDownload.Database;
    using StatsDownload.Database.CastleWindsor;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.DataStore;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.Parsing;
    using StatsDownload.SharpZipLib;
    using StatsDownload.Wrappers;
    using StatsDownload.Wrappers.Networking;

    public static class DependencyInjection
    {
        public static void ConfigureThisApp(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationLoggingService, FileDownloadConsoleLoggingProvider>()
                    .AddSingleton<IDateTimeService, DateTimeProvider>().AddSingleton<IFileService, FileProvider>()
                    .AddSingleton<IDirectoryService, DirectoryProvider>()
                    .AddSingleton<IResourceCleanupService, ResourceCleanupProvider>()
                    .AddSingleton<ILoggingService, LoggingProvider>()
                    .AddSingleton<IFileDownloadLoggingService, StatsDownloadLoggingProvider>()
                    .AddSingleton<IFilePayloadSettingsService, FilePayloadSettingsProvider>()
                    .AddSingleton<IFileCompressionService, Bz2CompressionProvider>()
                    .AddSingleton<IFileReaderService, FileReaderProvider>()
                    .AddSingleton<IStatsDownloadDatabaseParameterService, StatsDownloadDatabaseParameterProvider>()
                    .AddSingleton<IDatabaseConnectionService, MicrosoftSqlDatabaseConnectionProvider>()
                    .AddSingleton<ITypedFactoryComponentSelector, DatabaseFactoryComponentSelector>()
                    .AddSingleton<IDatabaseConnectionServiceFactory>()
                    .AddSingleton<IStatsDownloadDatabaseService, StatsDownloadDatabaseProvider>()
                    .AddSingleton<IFileDownloadDatabaseService, FileDownloadDatabaseProvider>()
                    .AddSingleton<ISecureFilePayloadService, SecureFilePayloadProvider>()
                    .AddSingleton<IDownloadService, SecureDownloadProvider>()
                    .AddSingleton<IDownloadService, DownloadProvider>()
                    .AddSingleton<IDownloadSettingsValidatorService, DownloadSettingsValidatorProvider>()
                    .AddSingleton<IAdditionalUserDataParserService, AdditionalUserDataParserProvider>()
                    .AddSingleton<IBitcoinAddressValidatorService, BitcoinAddressValidatorProvider>()
                    .AddSingleton<IFileDownloadService, FileDownloadProvider>()
                    .AddSingleton<IFileDownloadMinimumWaitTimeService, FileDownloadMinimumWaitTimeProvider>()
                    .AddSingleton<IErrorMessageService, ErrorMessageProvider>()
                    .AddSingleton<IFileDownloadEmailService, StatsDownloadEmailProvider>()
                    .AddSingleton<IEmailSettingsValidatorService, EmailSettingsValidatorProvider>()
                    .AddSingleton<IEmailService, EmailProvider>()
                    .AddSingleton<IFilePayloadUploadService, FilePayloadUploadProvider>()
                    .AddTransient<IWebClient, WebClientWrapper>().AddSingleton<IWebClientFactory>()
                    .AddSingleton<IDataStoreService, UncDataStoreProvider>()
                    .AddSingleton<IFileValidationService, FileValidationProvider>()
                    .AddSingleton<IStatsFileParserService, StatsFileParserProvider>()
                    .AddSingleton<IStatsFileDateTimeFormatsAndOffsetService, StatsFileDateTimeFormatsAndOffsetProvider
                    >().AddSingleton<IDataStoreServiceFactory>()
                    .AddSingleton<ITypedFactoryComponentSelector, DataStoreFactoryComponentSelector>();
        }
    }
}