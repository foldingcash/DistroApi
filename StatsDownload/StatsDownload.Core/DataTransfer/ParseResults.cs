namespace StatsDownload.Core
{
    using System.Collections.Generic;

    public class ParseResults
    {
        public ParseResults(List<UserData> usersData, List<FailedUserData> failedUsersData)
        {
            UsersData = usersData;
            FailedUsersData = failedUsersData;
        }

        public List<FailedUserData> FailedUsersData { get; private set; }

        public List<UserData> UsersData { get; private set; }
    }
}