namespace StatsDownloadApi.Interfaces.DataTransfer
{
    using System;

    public class FoldingUsersResult
    {
        public FoldingUsersResult(FoldingUser[] foldingUsers, DateTime startDateTime, DateTime endDateTime)
        {
            FoldingUsers = foldingUsers;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public DateTime EndDateTime { get; }

        public FoldingUser[] FoldingUsers { get; }

        public DateTime StartDateTime { get; }
    }
}