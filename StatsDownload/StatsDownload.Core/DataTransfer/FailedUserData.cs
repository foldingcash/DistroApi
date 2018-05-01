namespace StatsDownload.Core
{
    public class FailedUserData
    {
        public FailedUserData()
        {
        }

        public FailedUserData(int lineNumber, string data, RejectionReason rejectionReason)
            : this(lineNumber, data, rejectionReason, null)
        {
        }

        public FailedUserData(int lineNumber, string data, RejectionReason rejectionReason, UserData userdata)
        {
            LineNumber = lineNumber;
            Data = data;
            RejectionReason = rejectionReason;
            UserData = userdata;
        }

        public string Data { get; private set; }

        public int LineNumber { get; private set; }

        public RejectionReason RejectionReason { get; private set; }

        public UserData UserData { get; private set; }
    }
}