namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using LazyCache;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces;

    public class StatsDownloadApiDatabaseCacheProvider : IStatsDownloadApiDatabaseService
    {
        private readonly IAppCache cache;

        private readonly IStatsDownloadApiDatabaseService innerService;

        private readonly ILogger<StatsDownloadApiDatabaseCacheProvider> logger;

        private readonly DatabaseCacheSettings settings;

        public StatsDownloadApiDatabaseCacheProvider(ILogger<StatsDownloadApiDatabaseCacheProvider> logger,
                                                     IOptions<DatabaseCacheSettings> settings, IAppCache cache,
                                                     IStatsDownloadApiDatabaseService innerService)
        {
            this.logger = logger;
            this.settings = settings.Value;
            this.cache = cache;
            this.innerService = innerService;
        }

        public IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(() => innerService.GetValidatedFiles(startDate, endDate),
                DateTimeOffset.Now.AddDays(settings.Days).AddHours(settings.Hours).AddMinutes(settings.Minutes),
                $"{startDate}-{endDate}");
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return innerService.IsAvailable();
        }

        private T GetOrAdd<T>(Func<T> func, DateTimeOffset addHours, string key = null,
                              [CallerMemberName] string method = null)
        {
            return cache.GetOrAdd($"{method}-{key}", func, addHours);
        }
    }
}