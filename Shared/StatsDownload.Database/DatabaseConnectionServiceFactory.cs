namespace StatsDownload.Database
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Database.Wrappers;

    public class DatabaseConnectionServiceFactory : IDatabaseConnectionServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        private ILogger logger;

        public DatabaseConnectionServiceFactory(ILogger<DatabaseConnectionServiceFactory> logger,
                                                IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public IDatabaseConnectionService Create()
        {
            return serviceProvider.GetRequiredService<MicrosoftSqlDatabaseConnectionProvider>();
        }
    }
}