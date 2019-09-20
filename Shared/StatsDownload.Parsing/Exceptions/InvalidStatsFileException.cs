namespace StatsDownload.Parsing.Exceptions
{
    using System;

    public class InvalidStatsFileException : Exception
    {
        public InvalidStatsFileException()
        {
        }

        public InvalidStatsFileException(string message)
            : base(message)
        {
        }
    }
}