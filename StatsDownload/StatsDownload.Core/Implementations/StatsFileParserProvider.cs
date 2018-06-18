namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

            var downloadDateTime = ParseDownloadDateTime(fileLines);
            Parse(fileLines, usersData, failedUsersData);

            return new ParseResults(downloadDateTime, usersData, failedUsersData);
        }

        private string[] GetFileLines(string fileData)
        {
            return fileData?.Replace("\r\n", "\n").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsInvalidStatsFile(string[] fileLines)
        {
            return fileLines == null || fileLines.Length < 2 || !ValidDateTime(fileLines)
                   || fileLines[1] != Constants.StatsFile.ExpectedHeader;
        }

        private string GetDateTimeLine(string[] fileLines)
        {
            return fileLines[0];
        }

        private DateTime ParseDownloadDateTime(string[] fileLines)
        {
            TryParseDownloadDateTime(fileLines, out DateTime downloadDateTime);
            return downloadDateTime;
        }

        private bool TryParseDownloadDateTime(string[] fileLines, out DateTime downloadDateTime)
        {
            var rawDateTime = GetDateTimeLine(fileLines);
            bool parsed = DateTime.TryParseExact(rawDateTime, Constants.StatsFile.StandardDateTimeFormats,
                CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out downloadDateTime);
            if (parsed)
            {
                downloadDateTime = new DateTimeOffset(downloadDateTime, new TimeSpan(0, -8, 0, 0)).UtcDateTime;
            }
            else
            {
                parsed = DateTime.TryParseExact(rawDateTime, Constants.StatsFile.DaylightSavingsDateTimeFormats,
                    CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out downloadDateTime);
                if (parsed)
                {
                    downloadDateTime = new DateTimeOffset(downloadDateTime, new TimeSpan(0, -7, 0, 0)).UtcDateTime;
                }
            }

            return parsed;
        }

        private bool ValidDateTime(string[] fileLines)
        {
            return TryParseDownloadDateTime(fileLines, out DateTime downloadDateTime);
        }

        private bool IsInvalidUserData(string[] unparsedUserData)
        {
            return unparsedUserData.Length != 3 && unparsedUserData.Length != 4;
        }

        private void Parse(string[] fileLines, List<UserData> usersData, List<FailedUserData> failedUsersData)
        {
            for (var lineIndex = 2; lineIndex < fileLines.Length; lineIndex++)
            {
                string currentLine = fileLines[lineIndex];
                string[] unparsedUserData = currentLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (IsInvalidUserData(unparsedUserData))
                {
                    failedUsersData.Add(
                        new FailedUserData(lineIndex + 1, currentLine, RejectionReason.UnexpectedFormat));
                    continue;
                }

                if (TryParseUserData(unparsedUserData, out UserData userData))
                {
                    additionalUserDataParserService.Parse(userData);
                    usersData.Add(userData);
                    continue;
                }

                failedUsersData.Add(new FailedUserData(lineIndex + 1, currentLine, RejectionReason.FailedParsing,
                    userData));
            }
        }

        private bool TryParseUserData(string[] unparsedUserData, out UserData userData)
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

            userData = new UserData(name, totalPoints, totalWorkUnits, teamNumber);

            return totalPointsParsed && totalWorkUnitsParsed && teamNumberParsed;
        }
    }
}