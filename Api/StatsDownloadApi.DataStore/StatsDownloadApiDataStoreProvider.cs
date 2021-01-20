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

        public StatsDownloadApiDataStoreProvider(IDataStoreServiceFactory dataStoreServiceFactory,
                                                 IStatsDownloadApiDatabaseService databaseService,
                                                 IFileValidationService fileValidationService,
                                                 IFilePayloadApiSettingsService filePayloadApiSettingsService,
                                                 ILoggingService loggingService)
        {
            dataStoreService = dataStoreServiceFactory.Create();

            this.databaseService = databaseService;
            this.fileValidationService = fileValidationService;
            this.filePayloadApiSettingsService = filePayloadApiSettingsService;
            this.loggingService = loggingService;
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
            int length = lastFileResults.UsersData.Count();
            var foldingUsers = new List<FoldingUser>(length);

            foreach (UserData userData in lastFileResults.UsersData)
            {
                UserData previous = firstFileResults.UsersData.FirstOrDefault(user =>
                    user.Name == userData.Name && user.TeamNumber == userData.TeamNumber);
                if (previous is UserData)
                {
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

                    foldingUsers.Add(user);
                }
                else
                {
                    foldingUsers.Add(new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.TotalPoints, userData.TotalWorkUnits));
                }
            }

            loggingService.LogMethodFinished();
            return foldingUsers.ToArray();
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
            loggingService.LogVerbose(
                $"First File: DownloadId={firstFile.DownloadId} DownloadDateTime={firstFile.DownloadDateTime}");
            loggingService.LogVerbose(
                $"Last File: DownloadId={lastFile.DownloadId} DownloadDateTime={lastFile.DownloadDateTime}");
            Task<ParseResults> firstResultTask = GetValidatedFile(firstFile);
            Task<ParseResults> lastResultTask = GetValidatedFile(lastFile);
            ParseResults[] results = await Task.WhenAll(firstResultTask, lastResultTask);
            loggingService.LogMethodFinished();
            return results;
        }
    }
}