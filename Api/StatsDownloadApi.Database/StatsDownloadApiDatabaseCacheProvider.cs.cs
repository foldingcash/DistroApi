namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using LazyCache;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    using StatsDownloadApi.Interfaces;

    public class StatsDownloadApiDatabaseCacheProvider : IStatsDownloadApiDatabaseService
    {
        private readonly IAppCache cache;

        private readonly int cacheDurationInHours = 12;

        private readonly IStatsDownloadApiDatabaseService innerService;

        private readonly int minimalCacheDurationInMinutes = 5;

        public StatsDownloadApiDatabaseCacheProvider(IStatsDownloadApiDatabaseService innerService, IAppCache cache)
        {
            this.innerService = innerService;
            this.cache = cache;
        }

        public IList<ValidatedFile> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(() => innerService.GetValidatedFiles(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours), $"{startDate}-{endDate}");
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return GetOrAdd(() => innerService.IsAvailable(),
                DateTimeOffset.Now.AddMinutes(minimalCacheDurationInMinutes));
        }

        private T GetOrAdd<T>(Func<T> func, DateTimeOffset addHours, string key = null,
                              [CallerMemberName] string method = null)
        {
            return cache.GetOrAdd($"{method}-{key}", func, addHours);
        }
    }
}