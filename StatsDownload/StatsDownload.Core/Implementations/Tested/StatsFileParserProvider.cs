namespace StatsDownload.Core
{
    using System;
    using System.Collections.Generic;

    public class StatsFileParserProvider : IStatsFileParserService
    {
        private const string ExpectedHeader = @"name	newcredit	sum(total)	team";

        public List<UserData> Parse(string fileData)
        {
            var usersData = new List<UserData>();

            string[] fileLines = fileData?.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (fileLines == null || fileLines.Length < 3 || fileLines[1] != ExpectedHeader)
            {
                return usersData;
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

                usersData.Add(new UserData(name, totalPoints, totalWorkUnits, teamNumber));
            }

            return usersData;
        }
    }
}