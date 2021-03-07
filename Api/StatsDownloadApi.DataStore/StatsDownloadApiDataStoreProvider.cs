namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    public class StatsDownloadApiDataStoreProvider : IStatsDownloadApiDataStoreService
    {
        private readonly IStatsDownloadApiDatabaseService databaseService;

        private readonly IDataStoreService dataStoreService;

        private readonly IFilePayloadApiSettingsService filePayloadApiSettingsService;

        private readonly IFileValidationService fileValidationService;

        private readonly ILoggingService loggingService;

        private readonly IResourceCleanupService resourceCleanupService;

        public StatsDownloadApiDataStoreProvider(IDataStoreServiceFactory dataStoreServiceFactory,
                                                 IStatsDownloadApiDatabaseService databaseService,
                                                 IFileValidationService fileValidationService,
                                                 IFilePayloadApiSettingsService filePayloadApiSettingsService,
                                                 ILoggingService loggingService,
                                                 IResourceCleanupService resourceCleanupService)
        {
            dataStoreService = dataStoreServiceFactory.Create();

            this.databaseService = databaseService;
            this.fileValidationService = fileValidationService;
            this.filePayloadApiSettingsService = filePayloadApiSettingsService;
            this.loggingService = loggingService;
            this.resourceCleanupService = resourceCleanupService;
        }

        public async Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(startDate, endDate);
            ParseResults first = parsedFiles.First();
            ParseResults last = parsedFiles.Last();
            FoldingUser[] foldingUsers = GetFoldingUsers(first, last);
            loggingService.LogMethodFinished();
            return new FoldingUsersResult(foldingUsers, first.DownloadDateTime, last.DownloadDateTime);
        }

        public async Task<Member[]> GetMembers(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(startDate, endDate);
            Member[] members = GetMembers(parsedFiles.First(), parsedFiles.Last());
            loggingService.LogMethodFinished();
            return members;
        }

        public async Task<Team[]> GetTeams()
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);
            Team[] teams = GetTeams(parsedFiles.Last());
            loggingService.LogMethodFinished();
            return teams;
        }

        public Task<bool> IsAvailable()
        {
            return dataStoreService.IsAvailable();
        }

        private FoldingUser[] GetFoldingUsers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            loggingService.LogMethodInvoked();

            Dictionary<(string Name, long TeamNumber), UserData> first =
                firstFileResults.UsersData.ToDictionary(data => (data.Name, data.TeamNumber));

            int length = lastFileResults.UsersData.Count();
            var foldingUsers = new Dictionary<(string, long), FoldingUser>(length);

            foreach (UserData userData in lastFileResults.UsersData)
            {
                bool exists = first.ContainsKey((userData.Name, userData.TeamNumber));

                if (exists)
                {
                    UserData previous = first[(userData.Name, userData.TeamNumber)];
                    var user = new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.TotalPoints - previous.TotalPoints, userData.TotalWorkUnits - previous.TotalWorkUnits);

                    if (user.PointsGained < 0)
                    {
                        throw new InvalidDistributionStateException(
                            "Negative points earned was detected for a user. There may be an issue with the database state or the stat files download. Contact development");
                    }

                    if (user.WorkUnitsGained < 0)
                    {
                        throw new InvalidDistributionStateException(
                            "Negative work units earned was detected for a user. There may be an issue with the database state or the stat files download. Contact development");
                    }

                    foldingUsers[(userData.Name, userData.TeamNumber)] = user;
                }
                else
                {
                    foldingUsers.Add((userData.Name, userData.TeamNumber),
                        new FoldingUser(userData.FriendlyName, userData.BitcoinAddress, userData.TotalPoints,
                            userData.TotalWorkUnits));
                }
            }

            FoldingUser[] users = foldingUsers.Select(pair => pair.Value).ToArray();
            loggingService.LogMethodFinished();
            return users;
        }

        private Member[] GetMembers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            loggingService.LogMethodInvoked();
            var members = new List<Member>(lastFileResults.UsersData.Count());

            foreach (UserData lastUserData in lastFileResults.UsersData)
            {
                UserData firstUserData =
                    firstFileResults.UsersData.FirstOrDefault(user => user.Name == lastUserData.Name);

                if (firstUserData == default(UserData))
                {
                    members.Add(new Member(lastUserData.Name, lastUserData.FriendlyName, lastUserData.BitcoinAddress,
                        lastUserData.TeamNumber, 0, 0, lastUserData.TotalPoints, lastUserData.TotalWorkUnits));
                    continue;
                }

                members.Add(new Member(lastUserData.Name, lastUserData.FriendlyName, lastUserData.BitcoinAddress,
                    lastUserData.TeamNumber, firstUserData.TotalPoints, firstUserData.TotalWorkUnits,
                    lastUserData.TotalPoints - firstUserData.TotalPoints,
                    lastUserData.TotalWorkUnits - firstUserData.TotalWorkUnits));
            }

            loggingService.LogMethodFinished();
            return members.ToArray();
        }

        private Team[] GetTeams(ParseResults lastFileResults)
        {
            loggingService.LogMethodInvoked();
            var teams = new List<Team>();

            foreach (UserData userData in lastFileResults.UsersData)
            {
                long teamNumber = userData.TeamNumber;

                if (teams.Any(team => team.TeamNumber == teamNumber))
                {
                    continue;
                }

                // TODO: Get the team name
                teams.Add(new Team(teamNumber, ""));
            }

            loggingService.LogMethodFinished();
            return teams.ToArray();
        }

        private async Task<ParseResults> GetValidatedFile(ValidatedFile validatedFile)
        {
            loggingService.LogMethodInvoked();
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            await dataStoreService.DownloadFile(filePayload, validatedFile);
            ParseResults results = fileValidationService.ValidateFile(filePayload);
            var result = new FileDownloadResult(filePayload);
            resourceCleanupService.Cleanup(result);
            loggingService.LogMethodFinished();
            return results;
        }

        private async Task<ParseResults[]> GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);
            IOrderedEnumerable<ValidatedFile> orderedFiles = validatedFiles.OrderBy(file => file.DownloadDateTime);
            ValidatedFile firstFile = orderedFiles.First();
            ValidatedFile lastFile = orderedFiles.Last();
            loggingService.LogDebug(
                $"First File: DownloadId={firstFile.DownloadId} DownloadDateTime={firstFile.DownloadDateTime}");
            loggingService.LogDebug(
                $"Last File: DownloadId={lastFile.DownloadId} DownloadDateTime={lastFile.DownloadDateTime}");
            Task<ParseResults> firstResultTask = GetValidatedFile(firstFile);
            Task<ParseResults> lastResultTask = GetValidatedFile(lastFile);
            ParseResults[] results = await Task.WhenAll(firstResultTask, lastResultTask);
            loggingService.LogMethodFinished();
            return results;
        }
    }
}