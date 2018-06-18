namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IStatsFileParserService
    {
        ParseResults Parse(string fileData);
    }
}