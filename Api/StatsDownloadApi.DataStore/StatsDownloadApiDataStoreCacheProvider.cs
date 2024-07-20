namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using LazyCache;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreCacheProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IAppCache cache;

        private readonly IStatsDownloadApiDataStoreService innerService;

        private readonly ILogger logger;

        private readonly DataStoreCacheSettings settings;

        public StatsDownloadApiDataStoreCacheProvider(ILogger<StatsDownloadApiDataStoreCacheProvider> logger,
                                                      IOptions<DataStoreCacheSettings> settings, IAppCache cache,
                                                      IStatsDownloadApiDataStoreService innerService)
        {
            this.logger = logger;
            this.settings = settings.Value;
            this.cache = cache;
            this.innerService = innerService;
        }

        public Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate, FoldingUserTypes includeFoldingUserTypes)
        {
            return GetOrAdd(async () => await innerService.GetFoldingMembers(startDate, endDate, includeFoldingUserTypes),
                $"{startDate}-{endDate}-{includeFoldingUserTypes}");
        }

        public Task<Member[]> GetMembers(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd(async () => await innerService.GetMembers(startDate, endDate), $"{startDate}-{endDate}");
        }

        public Task<Team[]> GetTeams()
        {
            return GetOrAdd(async () => await innerService.GetTeams(), DateTimeOffset.Now.AddHours(settings.Hours));
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

        private async Task<T> GetOrAdd<T>(Func<Task<T>> func, string key = null,
                                          [CallerMemberName] string method = null)
        {
            return await cache.GetOrAdd($"{method}-{key}", func,
                       DateTimeOffset.Now.AddDays(settings.Days).AddHours(settings.Hours).AddMinutes(settings.Minutes));
        }
    }
}