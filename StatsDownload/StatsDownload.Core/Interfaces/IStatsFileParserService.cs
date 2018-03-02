namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public interface IStatsFileParserService
    {
        List<UserData> Parse(string fileData);
    }
}