namespace StatsDownload.Core
{
    public class UserData
    {
        public UserData()
        {
        }

        public UserData(string name, ulong totalPoints, ulong totalWorkUnits, ulong teamNumber)
        {
            Name = name;
            TotalPoints = totalPoints;
            TotalWorkUnits = totalWorkUnits;
            TeamNumber = teamNumber;
        }

        public string BitcoinAddress { get; set; }

        public string FriendlyName { get; set; }

        public string Name { get; private set; }

        public ulong TeamNumber { get; private set; }

        public ulong TotalPoints { get; private set; }

        public ulong TotalWorkUnits { get; private set; }
    }
}