﻿namespace StatsDownload.Parsing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Logging;

    public class StatsFileParserProvider : IStatsFileParserService
    {
        private readonly IAdditionalUserDataParserService additionalUserDataParserService;

        private readonly ILogger<StatsFileParserProvider> logger;

        private readonly IStatsFileDateTimeFormatsAndOffsetService statsFileDateTimeFormatsAndOffsetService;

        public StatsFileParserProvider(ILogger<StatsFileParserProvider> logger,
                                       IAdditionalUserDataParserService additionalUserDataParserService,
                                       IStatsFileDateTimeFormatsAndOffsetService
                                           statsFileDateTimeFormatsAndOffsetService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.additionalUserDataParserService = additionalUserDataParserService
                                                   ?? throw new ArgumentNullException(
                                                       nameof(additionalUserDataParserService));
            this.statsFileDateTimeFormatsAndOffsetService = statsFileDateTimeFormatsAndOffsetService
                                                            ?? throw new ArgumentNullException(
                                                                nameof(statsFileDateTimeFormatsAndOffsetService));
        }

        public ParseResults Parse(FilePayload filePayload)
        {
            logger.LogMethodInvoked();
            string fileData = filePayload.DecompressedDownloadFileData;

            string[] fileLines = GetFileLines(fileData);
            int length = fileLines?.Length ?? 0;

            var usersData = new UserData[length];
            var failedUsersData = new BlockingCollection<FailedUserData>();

            if (IsInvalidStatsFile(fileLines))
            {
                throw new InvalidStatsFileException();
            }

            DateTime downloadDateTime = ParseDownloadDateTime(fileLines);
            Parse(fileLines, length, usersData, failedUsersData);
            IEnumerable<UserData> filteredUsersData = usersData.Where(data => data != null);
            var results = new ParseResults(downloadDateTime, filteredUsersData.ToArray(), failedUsersData.ToArray());
            logger.LogMethodFinished();
            return results;
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

        private void Parse(string[] fileLines, int length, UserData[] usersData,
                           BlockingCollection<FailedUserData> failedUsersData)
        {
            Parallel.For(2, length, index =>
            {
                int lineNumber = index + 1;
                string currentLine = fileLines[index];
                string[] unparsedUserData = currentLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (IsInvalidUserData(unparsedUserData))
                {
                    failedUsersData.Add(new FailedUserData(lineNumber, currentLine, RejectionReason.UnexpectedFormat));
                    return;
                }

                if (TryParseUserData(lineNumber, unparsedUserData, out UserData userData))
                {
                    additionalUserDataParserService.Parse(userData);
                    usersData[index] = userData;
                    return;
                }

                failedUsersData.Add(
                    new FailedUserData(lineNumber, currentLine, RejectionReason.FailedParsing, userData));
            });
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
            downloadDateTime = default;

            foreach ((string format, int hourOffset) in statsFileDateTimeFormatsAndOffsetService
                .GetStatsFileDateTimeFormatsAndOffset())
            {
                parsed = DateTime.TryParseExact(rawDateTime, format, CultureInfo.CurrentCulture, DateTimeStyles.None,
                    out downloadDateTime);

                if (parsed)
                {
                    if (hourOffset == 0)
                    {
                        downloadDateTime = new DateTimeOffset(downloadDateTime).UtcDateTime;
                    }
                    else
                    {
                        downloadDateTime = new DateTimeOffset(downloadDateTime, new TimeSpan(0, hourOffset, 0, 0))
                            .UtcDateTime;
                    }

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
            return TryParseDownloadDateTime(fileLines, out DateTime _);
        }
    }
}