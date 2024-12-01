namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Microsoft.Extensions.Logging;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Logging;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IStatsDownloadApiDatabaseService databaseService;

        private readonly IDataStoreService dataStoreService;

        private readonly IFilePayloadApiSettingsService filePayloadApiSettingsService;

        private readonly IFileValidationService fileValidationService;

        private readonly ILogger logger;

        private readonly IResourceCleanupService resourceCleanupService;

        public StatsDownloadApiDataStoreProvider(IDataStoreServiceFactory dataStoreServiceFactory,
            IStatsDownloadApiDatabaseService databaseService,
            IFileValidationService fileValidationService,
            IFilePayloadApiSettingsService filePayloadApiSettingsService,
            ILogger<StatsDownloadApiDataStoreProvider> logger,
            IResourceCleanupService resourceCleanupService)
        {
            dataStoreService = dataStoreServiceFactory.Create();

            this.databaseService = databaseService;
            this.fileValidationService = fileValidationService;
            this.filePayloadApiSettingsService = filePayloadApiSettingsService;
            this.logger = logger;
            this.resourceCleanupService = resourceCleanupService;
        }

        public async Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate,
            FoldingUserTypes includeFoldingUserTypes)
        {
            logger.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(startDate, endDate);
            ParseResults first = parsedFiles.First();
            ParseResults last = parsedFiles.Last();
            FoldingUser[] foldingUsers = GetFoldingUsers(first, last);
            FoldingUser[] filteredFoldingUsers = FilterFoldingUsers(foldingUsers, includeFoldingUserTypes);
            logger.LogMethodFinished();
            return new FoldingUsersResult(filteredFoldingUsers, first.DownloadDateTime, last.DownloadDateTime);
        }

        public async Task<Member[]> GetMembers(DateTime startDate, DateTime endDate)
        {
            logger.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(startDate, endDate);
            Member[] members = GetMembers(parsedFiles.First(), parsedFiles.Last());
            logger.LogMethodFinished();
            return members;
        }

        public async Task<Team[]> GetTeams()
        {
            logger.LogMethodInvoked();
            ParseResults[] parsedFiles =
                await GetValidatedFiles(DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.Date);
            Team[] teams = GetTeams(parsedFiles.Last());
            logger.LogMethodFinished();
            return teams;
        }

        public Task<bool> IsAvailable()
        {
            return dataStoreService.IsAvailable();
        }

        private FoldingUser[] FilterFoldingUsers(FoldingUser[] foldingUsers, FoldingUserTypes includeFoldingUserTypes)
        {
            bool BitcoinFilter(FoldingUser f)
            {
                return includeFoldingUserTypes.HasFlag(FoldingUserTypes.Bitcoin) &&
                       !string.IsNullOrEmpty(f.BitcoinAddress);
            }

            bool BitcoinCashFilter(FoldingUser f)
            {
                return includeFoldingUserTypes.HasFlag(FoldingUserTypes.BitcoinCash) &&
                       !string.IsNullOrEmpty(f.BitcoinCashAddress);
            }

            bool SlpFilter(FoldingUser f)
            {
                return includeFoldingUserTypes.HasFlag(FoldingUserTypes.Slp) && !string.IsNullOrEmpty(f.SlpAddress);
            }

            bool CashTokensFilter(FoldingUser f)
            {
                return includeFoldingUserTypes.HasFlag(FoldingUserTypes.CashTokens) &&
                       !string.IsNullOrEmpty(f.CashTokensAddress);
            }

            return foldingUsers
                   .Where(f => BitcoinFilter(f) || BitcoinCashFilter(f) || SlpFilter(f) || CashTokensFilter(f))
                   .ToArray();
        }

        private FoldingUser[] GetFoldingUsers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();

            Dictionary<(string Name, long TeamNumber), UserData> first = firstFileResults.UsersData
                .ToDictionary(data => (data.Name, data.TeamNumber));

            int length = lastFileResults.UsersData.Length;
            var users = new BlockingCollection<FoldingUser>(length);

            Parallel.ForEach(lastFileResults.UsersData, userData =>
            {
                UserData previous;
                bool exists = first.TryGetValue((userData.Name, userData.TeamNumber), out previous);

                if (exists)
                {
                    var user = new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.BitcoinCashAddress, userData.SlpAddress, userData.CashTokensAddress,
                        userData.TotalPoints - previous.TotalPoints, userData.TotalWorkUnits - previous.TotalWorkUnits);

                    if (user.PointsGained < 0)
                    {
                        throw new InvalidDistributionStateException(
                            "Negative points earned was detected for a user. There may be an issue with the database state or the stat files download. Contact development.");
                    }

                    if (user.WorkUnitsGained < 0)
                    {
                        throw new InvalidDistributionStateException(
                            "Negative work units earned was detected for a user. There may be an issue with the database state or the stat files download. Contact development.");
                    }

                    users.Add(user);
                }
                else
                {
                    users.Add(new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.BitcoinCashAddress, userData.SlpAddress, userData.CashTokensAddress,
                        userData.TotalPoints,
                        userData.TotalWorkUnits));
                }
            });

            logger.LogMethodFinished();
            return users.ToArray();
        }

        private Member[] GetMembers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();
            var members = new BlockingCollection<Member>(lastFileResults.UsersData.Length);
            Dictionary<(string Name, long TeamNumber), UserData> firstFile =
                firstFileResults.UsersData.ToDictionary(u => (u.Name, u.TeamNumber), u => u);

            Parallel.ForEach(lastFileResults.UsersData, lastUserData =>
            {
                UserData firstUserData;
                bool existsInFirst =
                    firstFile.TryGetValue((lastUserData.Name, lastUserData.TeamNumber), out firstUserData);

                if (existsInFirst)
                {
                    members.Add(new Member(lastUserData.Name, lastUserData.FriendlyName, lastUserData.BitcoinAddress,
                        lastUserData.BitcoinCashAddress, lastUserData.SlpAddress, lastUserData.CashTokensAddress,
                        lastUserData.TeamNumber, firstUserData.TotalPoints, firstUserData.TotalWorkUnits,
                        lastUserData.TotalPoints - firstUserData.TotalPoints,
                        lastUserData.TotalWorkUnits - firstUserData.TotalWorkUnits));
                }
                else
                {
                    members.Add(new Member(lastUserData.Name, lastUserData.FriendlyName, lastUserData.BitcoinAddress,
                        lastUserData.BitcoinCashAddress, lastUserData.SlpAddress, lastUserData.CashTokensAddress,
                        lastUserData.TeamNumber, 0, 0, lastUserData.TotalPoints, lastUserData.TotalWorkUnits));
                }
            });

            logger.LogMethodFinished();
            return members.ToArray();
        }

        private Team[] GetTeams(ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();
            var teams = new ConcurrentDictionary<long, Team>();

            Parallel.ForEach(lastFileResults.UsersData, user =>
            {
                long teamNumber = user.TeamNumber;

                if (!teams.ContainsKey(teamNumber))
                {
                    // TODO: Get the team name
                    teams.TryAdd(teamNumber, new Team(teamNumber, ""));
                }
            });

            logger.LogMethodFinished();
            return teams.Values.ToArray();
        }

        private async Task<ParseResults> GetValidatedFile(ValidatedFile validatedFile)
        {
            logger.LogMethodInvoked();
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            await dataStoreService.DownloadFile(filePayload, validatedFile);
            ParseResults results = fileValidationService.ValidateFile(filePayload);
            var result = new FileDownloadResult(filePayload);
            resourceCleanupService.Cleanup(result);
            logger.LogMethodFinished();
            return results;
        }

        private async Task<ParseResults[]> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            logger.LogMethodInvoked();
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);
            IOrderedEnumerable<ValidatedFile> orderedFiles = validatedFiles.OrderBy(file => file.DownloadDateTime);
            ValidatedFile firstFile = orderedFiles.First();
            ValidatedFile lastFile = orderedFiles.Last();
            logger.LogDebug("First File: DownloadId={downloadId} DownloadDateTime={downloadDateTime}",
                firstFile.DownloadId, firstFile.DownloadDateTime);
            logger.LogDebug("Last File: DownloadId={downloadId} DownloadDateTime={downloadDateTime}",
                lastFile.DownloadId, lastFile.DownloadDateTime);

            var results = new Collection<ParseResults>();

            Task<ParseResults> first = GetValidatedFile(firstFile);
            Task<ParseResults> last = GetValidatedFile(lastFile);

            await Task.WhenAll(first, last);

            results.Add(await first);
            results.Add(await last);

            logger.LogMethodFinished();
            return results.ToArray();
        }
    }
}