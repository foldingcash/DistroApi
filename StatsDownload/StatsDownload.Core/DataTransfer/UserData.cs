namespace StatsDownload.Core
{
    public class UserData
    {
        public UserData()
            : this(null, 0, 0, 0)
        {
        }

        public UserData(string name, long totalPoints, long totalWorkUnits, long teamNumber)
        {
            Name = name;
            TotalPoints = totalPoints;
            TotalWorkUnits = totalWorkUnits;
            TeamNumber = teamNumber;
        }

        public string BitcoinAddress { get; set; }

        public string FriendlyName { get; set; }

        public string Name { get; }

        public long TeamNumber { get; }

        public long TotalPoints { get; }

        public long TotalWorkUnits { get; }
    }
}