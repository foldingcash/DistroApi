namespace StatsDownload.Core.Extensions
{
    using System;

    public static class StringExtensions
    {
        public static string SubstringSafe(this string source, int index, int maxLength)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                return source.Substring(index, Math.Min(source.Length, maxLength));
            }

            return string.Empty;
        }
    }
}