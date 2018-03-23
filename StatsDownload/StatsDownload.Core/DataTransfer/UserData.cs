namespace StatsDownload.Core
{
    public class UserData
    {
        public UserData()
        {
        }

        public UserData(string name, ulong totalPoints, int totalWorkUnits, int teamNumber)
        {
            Name = name;
            TotalPoints = totalPoints;
            TotalWorkUnits = totalWorkUnits;
            TeamNumber = teamNumber;
        }

        public string BitcoinAddress { get; set; }

        public string FriendlyName { get; set; }

        public string Name { get; private set; }

        public int TeamNumber { get; private set; }

        public ulong TotalPoints { get; private set; }

        public int TotalWorkUnits { get; private set; }
    }
}