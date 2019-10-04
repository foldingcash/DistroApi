namespace StatsDownloadApi.DataStore
{
    using System;

    using LazyCache;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreCacheProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IAppCache cache;

        private readonly IStatsDownloadApiDataStoreService innerService;

        public StatsDownloadApiDataStoreCacheProvider(IStatsDownloadApiDataStoreService innerService, IAppCache cache)
        {
            this.innerService = innerService;
            this.cache = cache;
        }

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            return cache.GetOrAdd($"{nameof(GetFoldingMembers)}-{startDate}-{endDate}",
                () => innerService.GetFoldingMembers(startDate, endDate), DateTimeOffset.Now.AddHours(8));
        }

        public Member[] GetMembers(DateTime startDate, DateTime endDate)
        {
            return cache.GetOrAdd($"{nameof(GetMembers)}-{startDate}-{endDate}",
                () => innerService.GetMembers(startDate, endDate), DateTimeOffset.Now.AddHours(8));
        }

        public Team[] GetTeams()
        {
            return cache.GetOrAdd($"{nameof(GetTeams)}", () => innerService.GetTeams(), DateTimeOffset.Now.AddHours(8));
        }

        public bool IsAvailable()
        {
            return cache.GetOrAdd(nameof(IsAvailable), () => innerService.IsAvailable(),
                DateTimeOffset.Now.AddMinutes(5));
        }
    }
}