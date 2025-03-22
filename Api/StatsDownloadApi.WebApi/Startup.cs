namespace StatsDownloadApi.WebApi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.Json.Serialization;
    using Core;
    using Database;
    using DataStore;
    using HealthChecks.UI.Client;
    using Interfaces;
    using LazyCache;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.DependencyInjection;

    public class Startup
    {
        private readonly IConfiguration configuration;

        private readonly ILogger<Startup> logger;

        private readonly SwaggerSettings swaggerSettings;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            var settings = new SwaggerSettings();
            configuration.GetSection(nameof(swaggerSettings)).Bind(settings);
            swaggerSettings = settings;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            logger.LogTrace("PID: {PID} Environment: {environment}", Process.GetCurrentProcess().Id,
                env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerSettings.JsonUrl, swaggerSettings.Name);
                c.RoutePrefix = swaggerSettings.SwaggerUrl;
            });

            app.UseHealthChecksUI(options =>
            {
                options.PageTitle = "API Health UI";
                options.UIPath = "/health-ui";
            });

            app.UseEndpoints(builder =>
            {
                builder.MapHealthChecks("/health");
                builder.MapHealthChecks("/health/details", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                builder.MapControllers();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                if (OperatingSystem.IsWindows())
                {
                    builder.AddEventLog();
                }
            });

            services.AddHealthChecks()
                    .AddSqlServer(configuration[$"{nameof(DatabaseSettings)}:ConnectionString"],
                        healthQuery: "select 1", name: "SQL Server", failureStatus: HealthStatus.Unhealthy,
                        tags: new[] { "Feedback", "Database" });

            services.AddHealthChecksUI(settings =>
                    {
                        settings.AddWebhookNotification("Discord",
                            configuration["Alerting:DiscordWebhook"],
                            "{ \"content\": \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\" }",
                            "{ \"content\": \"[[LIVENESS]] is back to life\" }");
                        settings.SetNotifyUnHealthyOneTimeUntilChange();
                        settings.SetEvaluationTimeInSeconds(10);
                        settings.MaximumHistoryEntriesPerEndpoint(60);
                        settings.SetApiMaxActiveRequests(1);
                        settings.AddHealthCheckEndpoint("FoldingCash API", "/health/details");
                    })
                    .AddInMemoryStorage();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerSettings.Version,
                    new OpenApiInfo { Title = swaggerSettings.Title, Version = swaggerSettings.Version });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddOptions();
            services.AddLazyCache();
            services.AddSingleton(configuration);

            services.AddStatsDownload(configuration);

            services.Configure<DataStoreCacheSettings>(
                configuration.GetSection($"{nameof(DataStoreSettings)}:{nameof(DataStoreCacheSettings)}"));

            services.Configure<DatabaseCacheSettings>(
                configuration.GetSection($"{nameof(DatabaseSettings)}:{nameof(DatabaseCacheSettings)}"));

            services.AddSingleton<IStatsDownloadApiEmailService, StatsDownloadApiEmailProvider>()
                    .AddSingleton<IStatsDownloadApiService, StatsDownloadApiProvider>()
                    .AddSingleton<IStatsDownloadApiTokenDistributionService, StandardTokenDistributionProvider>()
                    .AddSingleton<IFilePayloadApiSettingsService, FilePayloadApiSettingsProvider>();

            services.AddSingleton<IStatsDownloadApiDatabaseService>(provider =>
            {
                return new StatsDownloadApiDatabaseCacheProvider(
                    provider.GetRequiredService<ILogger<StatsDownloadApiDatabaseCacheProvider>>(),
                    provider.GetRequiredService<IOptions<DatabaseCacheSettings>>(),
                    provider.GetRequiredService<IAppCache>(),
                    new StatsDownloadApiDatabaseValidationProvider(new StatsDownloadApiDatabaseProvider(
                        provider.GetRequiredService<ILogger<StatsDownloadApiDatabaseProvider>>(),
                        provider.GetRequiredService<IStatsDownloadDatabaseService>())));
            });

            services.AddSingleton<IStatsDownloadApiDataStoreService>(provider =>
            {
                return new StatsDownloadApiDataStoreCacheProvider(
                    provider.GetRequiredService<ILogger<StatsDownloadApiDataStoreCacheProvider>>(),
                    provider.GetRequiredService<IOptionsMonitor<DataStoreCacheSettings>>(),
                    provider.GetRequiredService<IAppCache>(),
                    new StatsDownloadApiDataStoreProvider(provider.GetRequiredService<IDataStoreServiceFactory>(),
                        provider.GetRequiredService<IStatsDownloadApiDatabaseService>(),
                        provider.GetRequiredService<IFileValidationService>(),
                        provider.GetRequiredService<IFilePayloadApiSettingsService>(),
                        provider.GetRequiredService<ILogger<StatsDownloadApiDataStoreProvider>>(),
                        provider.GetRequiredService<IResourceCleanupService>()));
            });
        }
    }
}