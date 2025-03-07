namespace StatsDownloadApi.DataStore
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.DataTransfer;
    using LazyCache;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class StatsDownloadApiDataStoreCacheProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IAppCache cache;

        private readonly IStatsDownloadApiDataStoreService innerService;

        private readonly ILogger logger;

        private readonly IOptionsMonitor<DataStoreCacheSettings> settingsMonitor;

        public StatsDownloadApiDataStoreCacheProvider(ILogger<StatsDownloadApiDataStoreCacheProvider> logger,
            IOptionsMonitor<DataStoreCacheSettings> settingsMonitor, IAppCache cache,
            IStatsDownloadApiDataStoreService innerService)
        {
            this.logger = logger;
            this.settingsMonitor = settingsMonitor;
            this.cache = cache;
            this.innerService = innerService;
        }

        private DataStoreCacheSettings Settings => settingsMonitor.CurrentValue;

        public Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate,
            FoldingUserTypes includeFoldingUserTypes)
        {
            return GetOrAdd(
                $"{nameof(GetFoldingMembers)}-{startDate.ToFileTimeUtc()}-{endDate.ToFileTimeUtc()}-{includeFoldingUserTypes}",
                () => innerService.GetFoldingMembers(startDate, endDate, includeFoldingUserTypes));
        }

        public Task<Member[]> GetMembers(DateTime startDate, DateTime endDate)
        {
            return GetOrAdd($"{nameof(GetMembers)}-{startDate.ToFileTimeUtc()}-{endDate.ToFileTimeUtc()}",
                () => innerService.GetMembers(startDate, endDate));
        }

        public Task<Team[]> GetTeams()
        {
            return cache.GetOrAdd(nameof(GetTeams), () => innerService.GetTeams(), DateTimeOffset.Now.AddDays(1));
        }

        public Task<bool> IsAvailable()
        {
            return innerService.IsAvailable();
        }

        private Task<T> GetOrAdd<T>(string key, Func<Task<T>> func)
        {
            return GetOrAdd(key, func,
                DateTimeOffset.Now.AddDays(Settings.Days).AddHours(Settings.Hours).AddMinutes(Settings.Minutes));
        }

        private Task<T> GetOrAdd<T>(string key, Func<Task<T>> func, DateTimeOffset expires)
        {
            string file = Path.Combine(Settings.Directory, key) + ".json";
            return cache.GetOrAdd(key, async () =>
                {
                    if (File.Exists(file))
                    {
                        try
                        {
                            string contents = await File.ReadAllTextAsync(file);
                            return JsonSerializer.Deserialize<T>(contents);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to read {key} from cache", key);
                        }
                    }

                    T members = await func();
                    if (Directory.Exists(Settings.Directory))
                    {
                        try
                        {
                            string contents = JsonSerializer.Serialize(members);
                            await File.WriteAllTextAsync(file, contents);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to write {key} to cache", key);
                        }
                    }

                    return members;
                },
                expires);
        }
    }
}