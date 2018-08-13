namespace StatsDownloadApi.Interfaces.DataTransfer
{
    public class Team
    {
        public Team(long teamNumber, string teamName)
        {
            TeamNumber = teamNumber;
            TeamName = teamName;
        }

        public string TeamName { get; }

        public long TeamNumber { get; }
    }
}