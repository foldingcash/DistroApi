namespace StatsDownload.Core.Interfaces.DataTransfer
{
    using System;

    public class ParseResults
    {
        public ParseResults(DateTime downloadDateTime, UserData[] usersData, FailedUserData[] failedUsersData)
        {
            DownloadDateTime = downloadDateTime;
            UsersData = usersData;
            FailedUsersData = failedUsersData;
        }

        public DateTime DownloadDateTime { get; }

        public FailedUserData[] FailedUsersData { get; }

        public UserData[] UsersData { get; }
    }
}