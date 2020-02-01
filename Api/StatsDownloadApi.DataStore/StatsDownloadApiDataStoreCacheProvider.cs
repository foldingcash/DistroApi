namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using LazyCache;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreCacheProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IAppCache cache;

        private readonly int cacheDurationInHours = 12;

        private readonly IStatsDownloadApiDataStoreService innerService;

        public StatsDownloadApiDataStoreCacheProvider(IStatsDownloadApiDataStoreService innerService, IAppCache cache)
        {
            this.innerService = innerService;
            this.cache = cache;
        }

        public Task<FoldingUser[]> GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(async () => await innerService.GetFoldingMembers(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours), $"{startDate}-{endDate}");
        }

        public Task<Member[]> GetMembers(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(async () => await innerService.GetMembers(startDate, endDate),
                DateTimeOffset.Now.AddHours(cacheDurationInHours), $"{startDate}-{endDate}");
        }

        public Task<Team[]> GetTeams()
        {
            return GetOrAdd(async () => await innerService.GetTeams(),
                DateTimeOffset.Now.AddHours(cacheDurationInHours));
        }

        public async Task<bool> IsAvailable()
        {
            return await innerService.IsAvailable();
        }

        private async Task<T> GetOrAdd<T>(Func<Task<T>> func, DateTimeOffset addHours, string key = null,
                                          [CallerMemberName] string method = null)
        {
            return await cache.GetOrAdd($"{method}-{key}", func, addHours);
        }
    }
}