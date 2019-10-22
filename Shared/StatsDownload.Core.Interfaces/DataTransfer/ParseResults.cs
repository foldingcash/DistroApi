namespace StatsDownload.Core.Interfaces.DataTransfer
{
    using System;
    using System.Collections.Generic;

    public class ParseResults
    {
        public ParseResults(DateTime downloadDateTime, IEnumerable<UserData> usersData,
                            IEnumerable<FailedUserData> failedUsersData)
        {
            DownloadDateTime = downloadDateTime;
            UsersData = usersData;
            FailedUsersData = failedUsersData;
        }

        public DateTime DownloadDateTime { get; }

        public IEnumerable<FailedUserData> FailedUsersData { get; }

        public IEnumerable<UserData> UsersData { get; }
    }
}