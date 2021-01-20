namespace StatsDownload.Parsing
{
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Settings;

    public static class Constants
    {
        public static class BitcoinAddress
        {
            public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            public const int Size = 25;
        }

        public static class Download
        {
            public const int MaximumTimeout = 3600;

            public const int MaximumWaitTimeInHours = 100;

            public const int MinimumTimeout = 100;

            public const int MinimumWaitTimeInHours = 1;
        }

        public static class FilePayload
        {
            public const string DecompressedFileExtension = "txt";

            public const string DecompressedFileName = "daily_user_summary";

            public const string FileExtension = "bz2";

            public const string FileName = "daily_user_summary.txt";
        }

        public static class StatsFile
        {
            public static readonly string[] DateTimeFormats =
            {
                "ddd MMM  d HH:mm:ss {0} yyyy", "ddd MMM dd HH:mm:ss {0} yyyy"
            };

            public static readonly string[] ExpectedHeaders =
            {
                "name\tnewcredit\tsum(total)\tteam", "name\tscore\twu\tteam"
            };

            public static readonly DateTimeFormat[] TimeZonesAndOffset =
            {
                new DateTimeFormat { Format = "GMT", HourOffset = 0 },
                new DateTimeFormat { Format = "CDT", HourOffset = -5 },
                new DateTimeFormat { Format = "CST", HourOffset = -6 },
                new DateTimeFormat { Format = "PDT", HourOffset = -7 },
                new DateTimeFormat { Format = "PST", HourOffset = -8 }
            };
        }
    }
}