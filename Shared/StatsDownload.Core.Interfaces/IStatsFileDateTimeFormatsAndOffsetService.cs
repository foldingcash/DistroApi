namespace StatsDownload.Core.Interfaces
{
    public interface IStatsFileDateTimeFormatsAndOffsetService
    {
        (string format, int hourOffset)[] GetStatsFileDateTimeFormatsAndOffset();
    }
}