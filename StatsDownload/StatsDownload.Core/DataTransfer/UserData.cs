namespace StatsDownload.Core
{
    public class UserData
    {
        public UserData()
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

        public string Name { get; private set; }

        public long TeamNumber { get; private set; }

        public long TotalPoints { get; private set; }

        public long TotalWorkUnits { get; private set; }
    }
}