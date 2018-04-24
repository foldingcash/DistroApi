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

        public List<UserData> Parse(string fileData)
        {
            var usersData = new List<UserData>();

            string[] fileLines = fileData?.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines == null || fileLines.Length < 2 || !ValidDateTime(fileLines[0])
                || fileLines[1] != ExpectedHeader)
            {
                throw new InvalidStatsFileException();
            }

            for (var lineIndex = 2; lineIndex < fileLines.Length; lineIndex++)
            {
                string[] unparsedUserData = fileLines[lineIndex].Split(new[] { '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (unparsedUserData.Length != 4)
                {
                    continue;
                }

                string name = unparsedUserData[0];
                ulong totalPoints;
                int totalWorkUnits;
                int teamNumber;

                if (!ulong.TryParse(unparsedUserData[1], out totalPoints)
                    || !int.TryParse(unparsedUserData[2], out totalWorkUnits)
                    || !int.TryParse(unparsedUserData[3], out teamNumber))
                {
                    continue;
                }

                var userData = new UserData(name, totalPoints, totalWorkUnits, teamNumber);

                additionalUserDataParserService.Parse(userData);

                usersData.Add(userData);
            }

            return usersData;
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