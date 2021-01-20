namespace StatsDownload.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.Email;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureStatsDownload(this IServiceCollection serviceCollection,
                                                                IConfiguration configuration)
        {
            return serviceCollection.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)))
                                    .Configure<FilterSettings>(configuration.GetSection(nameof(FilterSettings)))
                                    .Configure<DateTimeSettings>(configuration.GetSection(nameof(DateTimeSettings)))
                                    .Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)))
                                    .Configure<DataStoreSettings>(configuration.GetSection(nameof(DataStoreSettings)));
        }
    }
}