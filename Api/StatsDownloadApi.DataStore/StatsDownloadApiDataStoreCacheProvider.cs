namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Runtime.CompilerServices;

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
            return GetOrAdd(() => innerService.GetFoldingMembers(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours), $"{startDate}-{endDate}");
        }

        public Member[] GetMembers(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(() => innerService.GetMembers(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours), $"{startDate}-{endDate}");
        }

        public Team[] GetTeams()
        {
            return GetOrAdd(() => innerService.GetTeams(), DateTimeOffset.Now.AddHours(cacheDurationInHours));
        }

        public bool IsAvailable()
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