namespace StatsDownload.Wrappers.Networking
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces.Networking;

    public class WebClientFactory : IWebClientFactory
    {
        private readonly ILogger<WebClientFactory> logger;

        private readonly IServiceProvider provider;

        public WebClientFactory(ILogger<WebClientFactory> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
        }

        public IWebClient Create()
        {
            logger.LogDebug("Creating a new web client");
            return provider.GetRequiredService<IWebClient>();
        }

        public void Release(IWebClient webClient)
        {
            logger.LogDebug("Releasing a web client");
        }
    }
}