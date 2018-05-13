namespace StatsDownload.Core.DataTransfer
{
    using System.Collections.Generic;

    public class ParseResults
    {
        public ParseResults(IEnumerable<UserData> usersData, IEnumerable<FailedUserData> failedUsersData)
        {
            UsersData = usersData;
            FailedUsersData = failedUsersData;
        }

        public IEnumerable<FailedUserData> FailedUsersData { get; }

        public IEnumerable<UserData> UsersData { get; }
    }
}