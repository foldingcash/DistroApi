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

            string[] fileLines = fileData?.Replace("\r\n", "\n")
                                          .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines == null || fileLines.Length < 2 || !ValidDateTime(fileLines[0])
                || fileLines[1] != ExpectedHeader)
            {
                throw new InvalidStatsFileException();
            }

            for (var lineIndex = 2; lineIndex < fileLines.Length; lineIndex++)
            {
                string currentLine = fileLines[lineIndex];
                string[] unparsedUserData = currentLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (unparsedUserData.Length != 3 && unparsedUserData.Length != 4)
                {
                    failedUsersData.Add(new FailedUserData(currentLine));
                    continue;
                }

                var index = 0;
                string name = unparsedUserData[index++];

                if (unparsedUserData.Length == 3)
                {
                    name = string.Empty;
                    index--;
                }

                long totalPoints;
                long totalWorkUnits;
                long teamNumber;

                bool totalPointsParsed = long.TryParse(unparsedUserData[index++], out totalPoints);
                bool totalWorkUnitsParsed = long.TryParse(unparsedUserData[index++], out totalWorkUnits);
                bool teamNumberParsed = long.TryParse(unparsedUserData[index++], out teamNumber);

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