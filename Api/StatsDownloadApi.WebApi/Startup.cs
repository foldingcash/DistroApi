namespace StatsDownloadApi.WebApi
{
    using System;

    using Castle.Windsor.MsDependencyInjection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using StatsDownloadApi.WebApi.CastleWindsor;

    using ApiWindsorContainer = StatsDownloadApi.WebApi.CastleWindsor.WindsorContainer;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(builder => builder.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
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

            IServiceProvider provider =
                WindsorRegistrationHelper.CreateServiceProvider(ApiWindsorContainer.Instance, services);
            DependencyRegistration.Register(); // This registration must come after the provider creation
            return provider;
        }
    }
}