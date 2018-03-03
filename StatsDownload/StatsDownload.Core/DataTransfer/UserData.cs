namespace StatsDownload.Core
{
    public class UserData
    {
        /// <summary>
        ///     The intent of this constructor is to be used during testing when one does not care about the values in the DTO
        ///     This constructor does not modify the properties, leaving them the default value
        /// </summary>
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

        public string Name { get; private set; }

        public int TeamNumber { get; private set; }

        public ulong TotalPoints { get; private set; }

        public int TotalWorkUnits { get; private set; }
    }
}