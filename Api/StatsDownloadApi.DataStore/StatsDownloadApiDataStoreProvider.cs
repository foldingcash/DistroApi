namespace StatsDownloadApi.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public FoldingUser[] GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = GetValidatedFiles(startDate, endDate);
            FoldingUser[] foldingUsers = GetFoldingUsers(parsedFiles.First(), parsedFiles.Last());
            loggingService.LogMethodFinished();
            return foldingUsers;
        }

        public Member[] GetMembers(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = GetValidatedFiles(startDate, endDate);
            Member[] members = GetMembers(parsedFiles.First(), parsedFiles.Last());
            loggingService.LogMethodFinished();
            return members;
        }

        public Team[] GetTeams()
        {
            loggingService.LogMethodInvoked();
            ParseResults[] parsedFiles = GetValidatedFiles(DateTime.MinValue, DateTime.MaxValue);
            Team[] teams = GetTeams(parsedFiles.Last());
            loggingService.LogMethodFinished();
            return teams;
        }

        public bool IsAvailable()
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
                UserData previous = firstFileResults.UsersData.FirstOrDefault(user => user.Name == userData.Name);
                if (previous is UserData)
                {
                    foldingUsers.Add(new FoldingUser(userData.FriendlyName, userData.BitcoinAddress,
                        userData.TotalPoints - previous.TotalPoints,
                        userData.TotalWorkUnits - previous.TotalWorkUnits));
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

        private ParseResults GetValidatedFile(ValidatedFile validatedFile)
        {
            loggingService.LogMethodInvoked();
            var filePayload = new FilePayload();
            filePayloadApiSettingsService.SetFilePayloadApiSettings(filePayload);
            dataStoreService.DownloadFile(filePayload, validatedFile);
            ParseResults results = fileValidationService.ValidateFile(filePayload);
            loggingService.LogMethodFinished();
            return results;
        }

        private ParseResults[] GetValidatedFiles(DateTime startDate, DateTime endDate)
        {
            loggingService.LogMethodInvoked();
            IList<ValidatedFile> validatedFiles = databaseService.GetValidatedFiles(startDate, endDate);
            IOrderedEnumerable<ValidatedFile> orderedFiles = validatedFiles.OrderBy(file => file.DownloadDateTime);
            ParseResults[] results = { GetValidatedFile(orderedFiles.First()), GetValidatedFile(orderedFiles.Last()) };
            loggingService.LogMethodFinished();
            return results;
        }
    }
}