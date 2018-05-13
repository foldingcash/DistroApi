namespace StatsDownload.Core.DataTransfer
{
    using StatsDownload.Core.Enums;

    public class FailedUserData
    {
        public FailedUserData()
            : this(0, null, RejectionReason.FailedParsing)
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

        public string Data { get; }

        public int LineNumber { get; }

        public RejectionReason RejectionReason { get; }

        public UserData UserData { get; }
    }
}