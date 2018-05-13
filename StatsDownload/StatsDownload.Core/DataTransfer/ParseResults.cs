namespace StatsDownload.Core.DataTransfer
{
    using System.Collections.Generic;

    public class ParseResults
    {
        public ParseResults(List<UserData> usersData, List<FailedUserData> failedUsersData)
        {
            UsersData = usersData;
            FailedUsersData = failedUsersData;
        }

        public IEnumerable<FailedUserData> FailedUsersData { get; }

        public IEnumerable<UserData> UsersData { get; }
    }
}