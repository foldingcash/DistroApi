namespace StatsDownload.Core
{
    public interface IStatsFileParserService
    {
        ParseResults Parse(string fileData);
    }
}