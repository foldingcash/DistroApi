namespace StatsDownloadApi.WebApi
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;

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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddOptions();
            services.AddLazyCache();
            services.AddSingleton(Configuration);

            services.AddStatsDownload(Configuration);

            services.AddSingleton<IApplicationLoggingService, StatsDownloadApiLoggingProvider>()
                    .AddSingleton<IStatsDownloadApiEmailService, StatsDownloadApiEmailProvider>();
        }
    }
}