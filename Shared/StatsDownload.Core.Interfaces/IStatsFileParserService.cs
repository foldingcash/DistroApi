namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IStatsFileParserService
    {
        ParseResults Parse(string fileData);
    }
}