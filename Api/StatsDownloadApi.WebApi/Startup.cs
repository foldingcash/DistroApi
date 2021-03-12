namespace StatsDownloadApi.WebApi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using LazyCache;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.DependencyInjection;

    using StatsDownloadApi.Core;
    using StatsDownloadApi.Database;
    using StatsDownloadApi.DataStore;
    using StatsDownloadApi.Interfaces;

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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerSettings.JsonUrl, swaggerSettings.Name);
                c.RoutePrefix = swaggerSettings.SwaggerUrl;
            });

            app.UseRouting();
            app.UseEndpoints(builder => builder.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
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
                    provider.GetRequiredService<IOptions<DataStoreCacheSettings>>(),
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