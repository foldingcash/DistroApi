namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class StatsFileParserProvider : IStatsFileParserService
    {
        private const string ExpectedHeader = @"name	newcredit	sum(total)	team";

        private readonly IAdditionalUserDataParserService additionalUserDataParserService;

        public StatsFileParserProvider(IAdditionalUserDataParserService additionalUserDataParserService)
        {
            this.additionalUserDataParserService = additionalUserDataParserService;
        }

        public ParseResults Parse(string fileData)
        {
            var usersData = new List<UserData>();
            var failedUsersData = new List<FailedUserData>();
            var parseResults = new ParseResults(usersData, failedUsersData);

            string[] fileLines = fileData?.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines == null || fileLines.Length < 2 || !ValidDateTime(fileLines[0])
                || fileLines[1] != ExpectedHeader)
            {
                throw new InvalidStatsFileException();
            }

            for (var lineIndex = 2; lineIndex < fileLines.Length; lineIndex++)
            {
                string currentLine = fileLines[lineIndex];
                string[] unparsedUserData = currentLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (unparsedUserData.Length != 4)
                {
                    failedUsersData.Add(new FailedUserData(currentLine));
                    continue;
                }

                string name = unparsedUserData[0];
                ulong totalPoints;
                int totalWorkUnits;
                int teamNumber;

                bool totalPointsParsed = ulong.TryParse(unparsedUserData[1], out totalPoints);
                bool totalWorkUnitsParsed = int.TryParse(unparsedUserData[2], out totalWorkUnits);
                bool teamNumberParsed = int.TryParse(unparsedUserData[3], out teamNumber);

                var userData = new UserData(name, totalPoints, totalWorkUnits, teamNumber);

                if (!totalPointsParsed || !totalWorkUnitsParsed || !teamNumberParsed)
                {
                    failedUsersData.Add(new FailedUserData(currentLine, userData));
                    continue;
                }

                additionalUserDataParserService.Parse(userData);

                usersData.Add(userData);
            }

            return parseResults;
        }

        private bool ValidDateTime(string dateTime)
        {
            DateTime parsedDateTime;
            var format = "ddd MMM dd HH:mm:ss PST yyyy";
            return DateTime.TryParseExact(dateTime, format, CultureInfo.CurrentCulture,
                DateTimeStyles.NoCurrentDateDefault, out parsedDateTime);
        }
    }
}