namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.DataTransfer;

    public interface IStatsFileParserService
    {
        ParseResults Parse(string fileData);
    }
}