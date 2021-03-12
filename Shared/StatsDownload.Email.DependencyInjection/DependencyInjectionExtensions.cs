namespace StatsDownload.Email.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddEmail(this IServiceCollection serviceCollection,
                                                  IConfiguration configuration)
        {
            return serviceCollection.AddOptions()
                                    .Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)))
                                    .AddSingleton<IEmailSettingsValidatorService, EmailSettingsValidatorProvider>()
                                    .AddSingleton<IEmailService, EmailProvider>();
        }
    }
}