namespace StatsDownload.Core.Implementations
{
    using Interfaces;

    public class StatsFileDateTimeFormatsAndOffsetProvider : IStatsFileDateTimeFormatsAndOffsetService
    {
        public (string format, int hourOffset)[] GetStatsFileDateTimeFormatsAndOffset()
        {
            return Constants.StatsFile.DateTimeFormatsAndOffset;
        }
    }
}