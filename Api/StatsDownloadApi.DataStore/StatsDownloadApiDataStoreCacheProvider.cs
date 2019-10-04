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
            return cache.GetOrAdd($"{startDate}-{endDate}", () => innerService.GetFoldingMembers(startDate, endDate), DateTimeOffset.Now.AddHours(8));
        }

        public Member[] GetMembers(DateTime minValue, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Team[] GetTeams()
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }
}