namespace StatsDownload.Parsing
{
    public static class Constants
    {
        public static class BitcoinAddress
        {
            public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

            public const int Size = 25;
        }

        public static class StatsFile
        {
            public static readonly string[] DateTimeFormats =
            {
                "ddd MMM  d HH:mm:ss {0} yyyy", "ddd MMM dd HH:mm:ss {0} yyyy"
            };

            public static readonly (string timeZone, int hourOffset)[] TimeZonesAndOffset =
            {
                ("GMT", 0), ("CDT", -5), ("CST", -6), ("PDT", -7), ("PST", -8)
            };

            public static readonly string[] ExpectedHeaders =
            {
                "name\tnewcredit\tsum(total)\tteam", "name\tscore\twu\tteam"
            };
        }
    }
}