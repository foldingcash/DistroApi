namespace StatsDownloadApi.DataStore
{
    using System;

    using LazyCache;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreCacheProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IAppCache cache;

        private readonly int cacheDurationInHours = 12;

        private readonly IStatsDownloadApiDataStoreService innerService;

        private readonly int minimalCacheDurationInMinutes = 5;

        public StatsDownloadApiDataStoreCacheProvider(IStatsDownloadApiDataStoreService innerService, IAppCache cache)
        {
            this.innerService = innerService;
            this.cache = cache;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            return cache.GetOrAdd($"{nameof(GetFoldingMembers)}-{startDate}-{endDate}",
                () => innerService.GetFoldingMembers(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours));
        }

        public Member[] GetMembers(DateTime startDate, DateTime endDate)
        {
            return cache.GetOrAdd($"{nameof(GetMembers)}-{startDate}-{endDate}",
                () => innerService.GetMembers(startDate, endDate), DateTimeOffset.Now.AddHours(cacheDurationInHours));
        }

        public Team[] GetTeams()
        {
            return cache.GetOrAdd($"{nameof(GetTeams)}", () => innerService.GetTeams(),
                DateTimeOffset.Now.AddHours(cacheDurationInHours));
        }

        public bool IsAvailable()
        {
            return cache.GetOrAdd(nameof(IsAvailable), () => innerService.IsAvailable(),
                DateTimeOffset.Now.AddMinutes(minimalCacheDurationInMinutes));
        }
    }
}