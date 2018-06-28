namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Exceptions;
    using Interfaces;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;

    public class StatsFileParserProvider : IStatsFileParserService
    {
        private readonly IAdditionalUserDataParserService additionalUserDataParserService;

        public StatsFileParserProvider(IAdditionalUserDataParserService additionalUserDataParserService)
        {
            if (additionalUserDataParserService == null)
            {
                throw new ArgumentNullException(nameof(additionalUserDataParserService));
            }

            this.additionalUserDataParserService = additionalUserDataParserService;
        }

        public ParseResults Parse(string fileData)
        {
            var usersData = new List<UserData>();
            var failedUsersData = new List<FailedUserData>();

            string[] fileLines = GetFileLines(fileData);

            if (IsInvalidStatsFile(fileLines))
            {
                throw new InvalidStatsFileException();
            }

            DateTime downloadDateTime = ParseDownloadDateTime(fileLines);
            Parse(fileLines, usersData, failedUsersData);

            return new ParseResults(downloadDateTime, usersData, failedUsersData);
        }

        private string GetDateTimeLine(string[] fileLines)
        {
            return fileLines[0];
        }

        private string[] GetFileLines(string fileData)
        {
            return fileData?.Replace("\r\n", "\n").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsInvalidStatsFile(string[] fileLines)
        {
            return fileLines == null || fileLines.Length < 2 || !ValidDateTime(fileLines)
                   || !Constants.StatsFile.ExpectedHeaders.Contains(fileLines[1]);
        }

        private bool IsInvalidUserData(string[] unparsedUserData)
        {
            return unparsedUserData.Length != 3 && unparsedUserData.Length != 4;
        }

        private void Parse(string[] fileLines, List<UserData> usersData, List<FailedUserData> failedUsersData)
        {
            for (var lineIndex = 2; lineIndex < fileLines.Length; lineIndex++)
            {
                int lineNumber = lineIndex + 1;
                string currentLine = fileLines[lineIndex];
                string[] unparsedUserData = currentLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (IsInvalidUserData(unparsedUserData))
                {
                    failedUsersData.Add(
                        new FailedUserData(lineNumber, currentLine, RejectionReason.UnexpectedFormat));
                    continue;
                }

                if (TryParseUserData(lineNumber, unparsedUserData, out UserData userData))
                {
                    additionalUserDataParserService.Parse(userData);
                    usersData.Add(userData);
                    continue;
                }

                failedUsersData.Add(new FailedUserData(lineNumber, currentLine, RejectionReason.FailedParsing,
                    userData));
            }
        }

        private DateTime ParseDownloadDateTime(string[] fileLines)
        {
            TryParseDownloadDateTime(fileLines, out DateTime downloadDateTime);
            return downloadDateTime;
        }

        private bool TryParseDownloadDateTime(string[] fileLines, out DateTime downloadDateTime)
        {
            string rawDateTime = GetDateTimeLine(fileLines);

            var parsed = false;
            downloadDateTime = default(DateTime);

            foreach ((string format, int hourOffset) in Constants.StatsFile.DateTimeFormatsAndOffset)
            {
                parsed = DateTime.TryParseExact(rawDateTime, format, CultureInfo.CurrentCulture,
                    DateTimeStyles.NoCurrentDateDefault, out downloadDateTime);

                if (parsed)
                {
                    downloadDateTime = new DateTimeOffset(downloadDateTime, new TimeSpan(0, hourOffset, 0, 0))
                        .UtcDateTime;
                    break;
                }
            }

            return parsed;
        }

        private bool TryParseUserData(int lineNumber, string[] unparsedUserData, out UserData userData)
        {
            var index = 0;
            string name = unparsedUserData[index];
            index++;

            if (unparsedUserData.Length == 3)
            {
                name = string.Empty;
                index--;
            }

            bool totalPointsParsed = long.TryParse(unparsedUserData[index], out long totalPoints);
            index++;
            bool totalWorkUnitsParsed = long.TryParse(unparsedUserData[index], out long totalWorkUnits);
            index++;
            bool teamNumberParsed = long.TryParse(unparsedUserData[index], out long teamNumber);

            userData = new UserData(lineNumber, name, totalPoints, totalWorkUnits, teamNumber);

            return totalPointsParsed && totalWorkUnitsParsed && teamNumberParsed;
        }

        private bool ValidDateTime(string[] fileLines)
        {
            return TryParseDownloadDateTime(fileLines, out DateTime downloadDateTime);
        }
    }
}