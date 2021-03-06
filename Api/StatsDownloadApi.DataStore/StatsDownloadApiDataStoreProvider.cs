﻿namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

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

        public async Task<FoldingUsersResult> GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            logger.LogMethodInvoked();
            ParseResults[] parsedFiles = await GetValidatedFiles(startDate, endDate);
            ParseResults first = parsedFiles.First();
            ParseResults last = parsedFiles.Last();
            FoldingUser[] foldingUsers = GetFoldingUsers(first, last);
            logger.LogMethodFinished();
            return new FoldingUsersResult(foldingUsers, first.DownloadDateTime, last.DownloadDateTime);
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
            ParseResults[] parsedFiles = await GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);
            Team[] teams = GetTeams(parsedFiles.Last());
            logger.LogMethodFinished();
            return teams;
        }

        public Task<bool> IsAvailable()
        {
            return dataStoreService.IsAvailable();
        }

        private FoldingUser[] GetFoldingUsers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();

            Dictionary<(string Name, long TeamNumber), UserData> first = firstFileResults.UsersData
                .Where(data => !string.IsNullOrEmpty(data.BitcoinAddress))
                .ToDictionary(data => (data.Name, data.TeamNumber));

            int length = lastFileResults.UsersData.Length;
            var users = new FoldingUser[length];

            Parallel.For(0, length, index =>
            {
                UserData userData = lastFileResults.UsersData[index];
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

                    users[index] = user;
                }
                else
                {
                    users[index] = new FoldingUser(userData.FriendlyName, userData.BitcoinAddress, userData.TotalPoints,
                        userData.TotalWorkUnits);
                }
            });

            logger.LogMethodFinished();
            return users.ToArray();
        }

        private Member[] GetMembers(ParseResults firstFileResults, ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();
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

            logger.LogMethodFinished();
            return members.ToArray();
        }

        private Team[] GetTeams(ParseResults lastFileResults)
        {
            logger.LogMethodInvoked();
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

            logger.LogMethodFinished();
            return teams.ToArray();
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

            var results = new BlockingCollection<ParseResults>();

            await Task.Run(() =>
            {
                Parallel.ForEach(new[] { firstFile, lastFile },
                    async file => { results.Add(await GetValidatedFile(file)); });
            });

            ParseResults[] ordered = results.OrderBy(file => file.DownloadDateTime).ToArray();

            logger.LogMethodFinished();
            return ordered;
        }
    }
}