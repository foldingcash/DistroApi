namespace StatsDownloadApi.WebApi
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.DependencyInjection;

    using StatsDownloadApi.Core;
    using StatsDownloadApi.Interfaces;

    public class Startup
    {
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            logger.LogTrace("PID: {PID} Environment: {environment}", Process.GetCurrentProcess().Id,
                env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(builder => builder.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddControllersWithViews();

            services.AddOptions();
            services.AddLazyCache();
            services.AddSingleton(Configuration);

            services.AddStatsDownload(Configuration);

            services.AddSingleton<IApplicationLoggingService, StatsDownloadApiLoggingProvider>()
                    .AddSingleton<IStatsDownloadApiEmailService, StatsDownloadApiEmailProvider>();
        }
    }
}