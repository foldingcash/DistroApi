namespace StatsDownload.DataStore
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;

    public class DataStoreServiceFactory : IDataStoreServiceFactory
    {
        private readonly IServiceProvider provider;

        public DataStoreServiceFactory(ILogger<DataStoreServiceFactory> logger, IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IDataStoreService Create()
        {
            return provider.GetRequiredService<UncDataStoreProvider>();
        }
    }
}